#include "game/updater.h"
#include "game/md5.h"
#include "network/httpObject.h"
#include "console/console.h"
#include <windows.h>

static const char* _ServerPath = "store.gginteractive.com:80";
static char _3SSDir[1024];

static HANDLE _hLogFile = INVALID_HANDLE_VALUE;

static void InitUpdateLog()
{
	// Nuke Backup log
	DeleteFileA("./update_old_old.log");

	MoveFileA("./update_old.log", "./update_old_old.log");

	// Backup existing log
	MoveFileA("./update.log", "./update_old.log");

	// Open new log
	_hLogFile = CreateFileA("./update.log", GENERIC_READ | GENERIC_WRITE, FILE_SHARE_READ, 0, CREATE_ALWAYS, 0, 0);
}

static void UpdateLog(const char* fmt, ...)
{
	if (_hLogFile != INVALID_HANDLE_VALUE)
	{
		va_list argptr;
		va_start(argptr, fmt);
		char buf[8192];
		dVsprintf(buf, sizeof(buf), fmt, argptr);
		strcat(buf, "\r\n");
		OutputDebugStringA(buf);
		WriteFile(_hLogFile, buf, strlen(buf), 0, 0);
		va_end(argptr);
	}
}

char** SplitString(const char* str, char delimiter, int &count)
{
	count = 0;
	int len = strlen(str);
	for (int i = 0; i < len; i++)
	{
		if (str[i] == delimiter)
			count++;
	}

	char** split = (char**)malloc(sizeof(char*) * count);
	count = 0;
	char* copy = _strdup(str);
	char* ptr = copy;
	for (int i = 0; i < len; i++)
	{
		if (copy[i] == delimiter)
		{
			copy[i] = 0;
			split[count++] = _strdup(ptr);
			ptr = &copy[i + 1];
		}
	}
	free(copy);
	return split;
}

class WebFile : public HTTPObject
{
public:
	WebFile()
	{
		_data = 0;
		_dataSize = 0;
		_finished = true;
	}

	~WebFile()
	{
		if (_data)
			free(_data);
	}

	void get(const char* serverAndPort, const char* path)
	{
		HTTPObject::get(serverAndPort, path, 0);
		_finished = false;
	}

	void finish()
	{
		while (!_finished)
		{
			Sleep(100);
		}
	}

	virtual void onConnectFailed()
	{
		HTTPObject::onConnectFailed();
		_finished = true;
	}

	virtual void onDisconnect()
	{
		HTTPObject::onDisconnect();
		_finished = true;
	}

	virtual U32 onReceive(U8 *buffer, U32 bufferLen)
	{
		if (mParseState == ProcessingBody)
		{
			int oldSize = _dataSize;
			_dataSize += bufferLen;

			_data = realloc(_data, _dataSize);
			U8* ptr = (U8*)_data + oldSize;
			memcpy(ptr, buffer, bufferLen);
			return bufferLen;
		}
		else
			return HTTPObject::onReceive(buffer, bufferLen);
	}

	bool _finished;
	void* _data;
	int _dataSize;
	char _url[2048];
};

struct ManifestEntry
{
	char* _file;
	char* _hash;

	ManifestEntry(char* line)
	{
		char* ptr = line;
		int len = strlen(line);
		for (int i = 0; i < len; i++)
		{
			if (line[i] == ',')
			{
				line[i] = 0;
				_file = _strdup(ptr);
				ptr = &line[i + 1];
			}
			if (line[i] == '\r')
			{
				line[i] = 0;
				_hash = _strdup(ptr);
				break;
			}
		}
	}

	~ManifestEntry()
	{
		free(_file);
		free(_hash);
	}
};

struct Manifest
{
	ManifestEntry** _entries;
	int _entryCount;

	Manifest(void* data, int dataLen)
	{
		char* datacopy = (char*)malloc(dataLen + 1);
		memcpy(datacopy, data, dataLen);
		datacopy[dataLen] = 0;

		_entryCount = 0;
		char* line = strtok((char*)datacopy, "\n");
		while (line)
		{
			_entryCount++;
			line = strtok(0, "\n");
		}

		free(datacopy);
		datacopy = (char*)malloc(dataLen + 1);
		memcpy(datacopy, data, dataLen);
		datacopy[dataLen] = 0;

		_entries = (ManifestEntry**)malloc(sizeof(ManifestEntry*) * _entryCount);
		int idx = 0;
		line = strtok((char*)datacopy, "\n");
		while (line)
		{
			_entries[idx++] = new ManifestEntry(line);
			line = strtok(0, "\n");	
		}
		free(datacopy);
	}

	~Manifest()
	{
		for (int i = 0; i < _entryCount; i++)
		{
			delete _entries[i];
		}
		free(_entries);		
	}
};

void CompareManifests(Manifest* om, Manifest* nm, HANDLE hDelFiles, DWORD& delFilesCount, HANDLE hUpdateFiles, DWORD& updateFilesCount, ManifestEntry** downloads)
{
	delFilesCount = 0;
	updateFilesCount = 0;

	// Compare old manifest to new manifest
	for (int i = 0; i < om->_entryCount; i++)
	{
		bool found = false;
		bool match = false;
		ManifestEntry* oe = om->_entries[i];
		ManifestEntry* ne = 0;
		for (int j = 0; j < nm->_entryCount; j++)
		{
			ne = nm->_entries[j];
			if (!_stricmp(oe->_file, ne->_file))
			{
				found = true;
				if (!_stricmp(oe->_hash, ne->_hash))
				{
					match = true;
				}
				break;
			}
		}
		
		if (found)
		{
			if (!match)
			{
				// File found but hashes dont match, mark for update
				if (hUpdateFiles)
				{
					WriteFile(hUpdateFiles, oe->_file, strlen(oe->_file), 0, 0);
					WriteFile(hUpdateFiles, "\n", 1, 0, 0);

					// mark for download
					downloads[updateFilesCount] = ne;
				}
				updateFilesCount++;
			}
		}
		else
		{
			// File in old manifest does not exist in new manifest
			delFilesCount++;
			if (hDelFiles)
			{
				WriteFile(hDelFiles, oe->_file, strlen(oe->_file), 0, 0);
				WriteFile(hDelFiles, "\n", 1, 0, 0);
			}
		}
	}

	// Compare new manifest to old manifest
	for (int i = 0; i < nm->_entryCount; i++)
	{
		bool found = false;
		ManifestEntry* ne = nm->_entries[i];
		for (int j = 0; j < om->_entryCount; j++)
		{
			ManifestEntry* oe = om->_entries[j];
			if (!_stricmp(oe->_file, ne->_file))
			{
				found = true;
				break;
			}
		}

		if (!found)
		{
			// File is in new manifest but not old manifest, its a new file.  Queue it for update
			if (hUpdateFiles)
			{
				WriteFile(hUpdateFiles, ne->_file, strlen(ne->_file), 0, 0);
				WriteFile(hUpdateFiles, "\n", 1, 0, 0);

				downloads[updateFilesCount] = ne;
			}
			updateFilesCount++;
		}
	}
}

U8* Hash(U8* data, int dataLen)
{
	MD5_CTX ctx;
	MD5Init(&ctx);
	MD5Update(&ctx, data, dataLen);
	MD5Final(&ctx);

	U8* hash = (U8*)malloc(16);
	memcpy(hash, ctx.digest, 16);
	return hash;
}

U8* HashFile(const char* file)
{
	HANDLE hFile = CreateFileA(file, GENERIC_READ, FILE_SHARE_READ, NULL, OPEN_EXISTING, 0, 0);
	int size = GetFileSize(hFile, 0);
	U8* data = (U8*)malloc(size);
	ReadFile(hFile, data, size, 0, 0);
	CloseHandle(hFile);

	U8* hash = Hash(data, size);
	free(data);
	return hash;
}

bool HashCompare(U8* hashA, U8* hashB)
{
	for (int i = 0; i < 16; i++)
	{
		if (hashA[i] != hashB[i])
			return false;
	}
	return true;
}

char* Hash2String(U8* hash)
{
	char temp[4];
	char* str = (char*)calloc(33, 1);
	for (int i = 0; i < 16; i++)
	{
		dSprintf(temp, sizeof(temp), "%02X", hash[i]);
		strcat(str, temp);
	}
	for (int i = 0; i < 33; i++)
		str[i] = tolower(str[i]);
	return str;
}

void ValidateDirectory(const char* file)
{
	char* path = _strdup(file);
	int len = strlen(path);
	for (int i = 0; i < len; i++)
	{
		if (path[i] == '\\')
			path[i] = '/';
	}
	for (int i = len - 1; i > 0; i--)
	{
		if (path[i] == '/')
		{
			path[i] = 0;
			break;
		}
	}


	char builtPath[1024];
	builtPath[0] = 0;
	char* dir = strtok(path, "/");
	while (dir)
	{
		strcat(builtPath, dir);
		strcat(builtPath, "/");
		if (GetFileAttributesA(builtPath) == INVALID_FILE_ATTRIBUTES)
		{
			CreateDirectoryA(builtPath, 0);
		}		
		dir = strtok(0, "/");
	}

	free(path);
}

int RemoveDirectory(const char* dir) // Fully qualified name of the directory being   deleted,   without trailing backslash
{
	int len = strlen(dir) + 2;
	char* tempdir = (char*)malloc(len);
	memset(tempdir, 0, len);
	strcpy(tempdir, dir);

	SHFILEOPSTRUCTA file_op = {
		NULL,
		FO_DELETE,
		tempdir,
		"",
		FOF_NOCONFIRMATION |
		FOF_NOERRORUI |
		FOF_SILENT,
		false,
		0,
		"" };
	int ret = SHFileOperationA(&file_op);
	free(tempdir);
	return ret; // returns 0 on success, non zero on failure.
}

void Crash()
{
	MessageBox(0, L"failed", L"failed", 0);
	int* crash = 0;
	int crashhere = crash[0];
}

void LoadManifestFile(U8** mainifestData, DWORD* manifestSize)
{
	char localManifestFile[1024];
	strcpy(localManifestFile, _3SSDir);
	strcat(localManifestFile, "/manifest.txt");

	DWORD attribs = GetFileAttributesA(localManifestFile);
	if (attribs != INVALID_FILE_ATTRIBUTES)
	{
		HANDLE hFile = CreateFileA(localManifestFile, GENERIC_READ, FILE_SHARE_READ, NULL, OPEN_EXISTING, 0, 0);
		if (hFile != INVALID_HANDLE_VALUE)
		{
			*manifestSize = GetFileSize(hFile, 0);
			*mainifestData = (U8*)malloc(*manifestSize);
			ReadFile(hFile, *mainifestData, *manifestSize, 0, 0);
			CloseHandle(hFile);
		}
	}
}

void ManifestProcessDirectory(const char* directory, HANDLE hManifest)
{
	char dir[2048];
	strcpy(dir, directory);
	strcat(dir, "/*.*");

	WIN32_FIND_DATAA findData;
	HANDLE hFind = FindFirstFileA(dir, &findData);
	
	// Files first
	do
	{
		char relPath[2048];
		strcpy(relPath, directory);
		strcat(relPath, "\\");
		strcat(relPath, findData.cFileName);
		if (findData.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY)
		{
			continue;
		}
		else
		{
			if (strstr(relPath, ".log") || strstr(relPath, "manifest.txt") || strstr(relPath, "uninstall.exe") || strstr(relPath, "lua5.1.dll"))
				continue;

			// Read in file data
			HANDLE hFile = CreateFileA(relPath, GENERIC_READ, FILE_SHARE_READ, NULL, OPEN_EXISTING, 0, 0);
			U8* data = (U8*)malloc(findData.nFileSizeLow);
			ReadFile(hFile, data, findData.nFileSizeLow, 0, 0);			
			CloseHandle(hFile);

			// Hash the data
			U8* hash = Hash(data, findData.nFileSizeLow);
			char* hashStr = Hash2String(hash);
			free(data);
			free(hash);

			// Write the hash to the manifest file
			char line[2048];
			strcpy(line, relPath);
			strcat(line, ",");
			strcat(line, hashStr);
			strcat(line, "\r\n");
			WriteFile(hManifest, line, strlen(line), 0, 0);
			
			free(hashStr);
		}
	} while (FindNextFileA(hFind, &findData));

	// Now directories
	hFind = FindFirstFileA(dir, &findData);
	do
	{
		char relPath[2048];
		strcpy(relPath, directory);
		strcat(relPath, "\\");
		strcat(relPath, findData.cFileName);
		if (findData.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY)
		{
			if (findData.cFileName[0] != '.' && !strstr(findData.cFileName, "Uninstall"))
				ManifestProcessDirectory(relPath, hManifest);
		}
	} while (FindNextFileA(hFind, &findData));
}

void GenerateManifest()
{
	char localManifestFile[1024];
	strcpy(localManifestFile, _3SSDir);
	strcat(localManifestFile, "/manifest.txt");

	HANDLE hFile = CreateFileA(localManifestFile, GENERIC_READ | GENERIC_WRITE, 0, 0, CREATE_ALWAYS, 0, 0);

	ManifestProcessDirectory(".", hFile);

	CloseHandle(hFile);
}

DWORD WINAPI UpdateCheckThread(void* param)
{
	// Fetch manifest from the server
	WebFile* web = new WebFile();
	web->get(_ServerPath, "/download/3ss/manifest.txt");
	
	// Read in local manifest if it exists
	U8* localManifest = 0;
	DWORD localManifestSize = 0;
	LoadManifestFile(&localManifest, &localManifestSize);	

	if (!localManifest)
	{
		// Generate a manifest now
		UpdateLog("Generating Local Manifest");
		GenerateManifest();
		LoadManifestFile(&localManifest, &localManifestSize);
	}

	// Wait for web request to finish
	UpdateLog("Waiting for web manifest");
	web->finish();
	
	// Compare local manifest to remote manifest
	U8* webHash = Hash((U8*)web->_data, web->_dataSize);
	U8* locHash = Hash(localManifest, localManifestSize);
	if (!HashCompare(webHash, locHash))
	{
		// Manifests dont match, build a change list
		UpdateLog("Manifest mismatch, testing for actual changes...");
		Manifest* manWeb = new Manifest(web->_data, web->_dataSize);
		Manifest* manLoc = new Manifest(localManifest, localManifestSize);
		DWORD delFileCount;
		DWORD updateFileCount;
		CompareManifests(manLoc, manWeb, 0, delFileCount, 0, updateFileCount, 0);
		if (delFileCount > 0 || updateFileCount > 0)
		{
			UpdateLog("%d files deleted, %d files updated", delFileCount, updateFileCount);

			// Create update dir
			char dldir[1024];
			GetTempPathA(sizeof(dldir), dldir);
			strcat(dldir, "/3SSUpdate_DL/");
			CreateDirectoryA(dldir, 0);
			HANDLE hDelFiles = 0;
			HANDLE hUpdateFiles = 0;

			char fileName[1024];
			if (delFileCount > 0)
			{
				strcpy(fileName, dldir);
				strcat(fileName, "delfiles.txt");
				hDelFiles = CreateFileA(fileName, GENERIC_READ | GENERIC_WRITE, 0, 0, CREATE_ALWAYS, 0, 0);
			}

			if (updateFileCount > 0)
			{
				strcpy(fileName, dldir);
				strcat(fileName, "updatefiles.txt");
				hUpdateFiles = CreateFileA(fileName, GENERIC_READ | GENERIC_WRITE, 0, 0, CREATE_ALWAYS, 0, 0);
			}
			
			// Build the actual update files and kick off downloads
			UpdateLog("Creating file lists");
			ManifestEntry** downloads = (ManifestEntry**)malloc(sizeof(ManifestEntry*) * updateFileCount);
			CompareManifests(manLoc, manWeb, hDelFiles, delFileCount, hUpdateFiles, updateFileCount, downloads);
			CloseHandle(hDelFiles);
			CloseHandle(hUpdateFiles);

			// Wait for downloads
			UpdateLog("Downloading files");
			for (DWORD i = 0; i < updateFileCount; i++)
			{
				ManifestEntry* dlentry = downloads[i];

				// Check to see if file is already in the dl directory
				char localFile[1024];
				strcpy(localFile, dldir);
				strcat(localFile, dlentry->_file);
				if (GetFileAttributesA(localFile) != INVALID_FILE_ATTRIBUTES)
				{
					// Local file exists already, check the hash
					UpdateLog("(%d/%d) - Downloaded file %s already exists", i + 1, updateFileCount, dlentry->_file);
					U8* hash = HashFile(localFile);
					char* str = Hash2String(hash);
					if (_stricmp(str, dlentry->_hash))
					{
						// Hash doesnt match, delete the local file
						DeleteFileA(localFile);
						UpdateLog("(%d/%d) - Downloaded file %s hash does not match", i + 1, updateFileCount, dlentry->_file);
					}
					free(hash);
					free(str);
				}

				// Now download if the file isnt already downloaded
				if (GetFileAttributesA(localFile) == INVALID_FILE_ATTRIBUTES)
				{
					WebFile* dlfile = new WebFile();
					strcpy(dlfile->_url, "/download/3ss/");
					strcat(dlfile->_url, &dlentry->_file[2]);
					int len = strlen(dlfile->_url);
					for (int j = 13; j < len; j++)
					{
						if (dlfile->_url[j] == '\\')
							dlfile->_url[j] = '/';
					}

					dlfile->get(_ServerPath, dlfile->_url);
					UpdateLog("(%d/%d) - Downloading file %s", i + 1, updateFileCount, dlentry->_file);
					dlfile->finish();

					// Hash the result to check for good download
					U8* fileHash = Hash((U8*)dlfile->_data, dlfile->_dataSize);
					char* fileHashStr = Hash2String(fileHash);
					if (!_stricmp(fileHashStr, dlentry->_hash))
					{
						ValidateDirectory(localFile);
						HANDLE hFile = CreateFileA(localFile, GENERIC_READ | GENERIC_WRITE, 0, 0, CREATE_ALWAYS, 0, 0);
						WriteFile(hFile, dlfile->_data, dlfile->_dataSize, 0, 0);
						CloseHandle(hFile);
					}
					else
					{
						UpdateLog("(%d/%d) - bad hash: %s", i + 1, updateFileCount, dlentry->_file);
						i--;
					}
					free(fileHash);
					free(fileHashStr);
					delete dlfile;
				}
			}
			free(downloads);

			// Push the update
			char updateDir[1024];
			GetTempPathA(sizeof(updateDir), updateDir);
			strcat(updateDir, "/3SSUpdate/");
			BOOL success = MoveFileA(dldir, updateDir);
			DWORD err = GetLastError();
			UpdateLog("Update Ready");
		}
		else
			UpdateLog("No actual changes to udpate");
		delete manWeb;
		delete manLoc;
	}
	else
		UpdateLog("Local hash matches web hash, no update to do");
	free(webHash);
	free(locHash);
	delete web;
	
	return 0;
}

void Updater::Init(const char** argv, int argc)
{	
	InitUpdateLog();

	bool qaMode = false;
	for (int i = 1; i < argc; i++)
	{
		if (!_stricmp(argv[i], "-qa"))
		{
			_ServerPath = "qa.store.gginteractive.com:80";
			UpdateLog("Using QA server for updates");
			qaMode = true;
			break;
		}
	}
	if (!qaMode && argc > 1)
		return;

	GetCurrentDirectoryA(sizeof(_3SSDir), _3SSDir);

	// Check for pending update
	char updateDir[1024];
	GetTempPathA(sizeof(updateDir), updateDir);
	strcat(updateDir, "/3SSUpdate/");
	
	DWORD attrib = GetFileAttributesA(updateDir);
	if (attrib != INVALID_FILE_ATTRIBUTES && (attrib & FILE_ATTRIBUTE_DIRECTORY))
	{
		// There is a pending update
		ApplyUpdate(updateDir, qaMode);
	}
	else
	{
		// Start update check thread
		HANDLE hThread = CreateThread(0, 0, UpdateCheckThread, 0, 0, 0);
	}
}

void Updater::ApplyUpdate(const char* updateDir, bool qaMode)
{
	UpdateLog("Applying Update...");

	// Delete any files that need to be deleted
	char delFiles[1024];
	strcpy(delFiles, updateDir);
	strcat(delFiles, "delfiles.txt");
	DWORD attribs = GetFileAttributesA(delFiles);
	if (attribs != INVALID_FILE_ATTRIBUTES)
	{
		HANDLE hFile = CreateFileA(delFiles, GENERIC_READ, FILE_SHARE_READ, NULL, OPEN_EXISTING, 0, 0);
		if (hFile != INVALID_HANDLE_VALUE)
		{
			DWORD size = GetFileSize(hFile, 0);
			
			char* data = (char*)malloc(size + 1);
			ReadFile(hFile, data, size, 0, 0);
			CloseHandle(hFile);
			data[size] = 0;

			int count = 0;
			char** delFileList = SplitString(data, '\n', count);
			for (int i = 0; i < count; i++)
			{
				char path[1024];
				strcpy(path, _3SSDir);
				strcat(path, delFileList[i]);
				BOOL success = DeleteFileA(path);
				UpdateLog("DeleteFile(%s) - %d", path, success);

				free(delFileList[i]);
			}
			free(delFileList);
			free(data);
		}
	}

	// Copy over updated files
	strcpy(delFiles, updateDir);
	strcat(delFiles, "updatefiles.txt");
	attribs = GetFileAttributesA(delFiles);
	if (attribs != INVALID_FILE_ATTRIBUTES)
	{
		HANDLE hFile = CreateFileA(delFiles, GENERIC_READ, FILE_SHARE_READ, NULL, OPEN_EXISTING, 0, 0);
		if (hFile != INVALID_HANDLE_VALUE)
		{
			DWORD size = GetFileSize(hFile, 0);

			char* data = (char*)malloc(size + 1);
			ReadFile(hFile, data, size, 0, 0);
			CloseHandle(hFile);
			data[size] = 0;

			int count = 0;
			char** updateFileList = SplitString(data, '\n', count);
			for (int i = 0; i < count; i++)
			{
				char* upfile = updateFileList[i];

				char srcFile[1024];
				strcpy(srcFile, updateDir);
				strcat(srcFile, upfile);

				DWORD err = 0;

				ValidateDirectory(upfile);
				if (!CopyFileA(srcFile, upfile, FALSE))
				{
					err = GetLastError();
					char usfile[1024];
					strcpy(usfile, upfile);
					strcat(usfile, "_");
					BOOL success = DeleteFileA(usfile);
					err = GetLastError();
					success = MoveFileA(upfile, usfile);
					err = GetLastError();
					UpdateLog("RenameFile(%s) - (%d)", upfile, err);
					success = CopyFileA(srcFile, upfile, FALSE);
					if (!success)
					{
						err = GetLastError();
						UpdateLog("UpdateFile(%s) - failed(%d)", upfile, err);
					}
					else
					{
						UpdateLog("UpdateFile(%s) - success", upfile);
					}
				}
				else
				{
					UpdateLog("UpdateFile(%s) - success", upfile);
				}
				free(updateFileList[i]);
			}
			free(updateFileList);
			free(data);
		}
	}
	
	// Delete update files
	UpdateLog("Deleting update directory...");
	RemoveDirectory(updateDir);

	// Delete local manifest file so that it gets regenerated
	char manifestFile[1024];
	strcpy(manifestFile, _3SSDir);
	strcat(manifestFile, "/manifest.txt");
	BOOL success = DeleteFileA(manifestFile);
	DWORD err = GetLastError();
	UpdateLog("Delete local manifest - %d", err);

	// Restart
	char exePath[1024];
	GetModuleFileNameA(NULL, exePath, sizeof(exePath));
	STARTUPINFOA si;
	PROCESS_INFORMATION pi;

	ZeroMemory(&si, sizeof(si));
	si.cb = sizeof(si);
	ZeroMemory(&pi, sizeof(pi));
	UpdateLog("Launching exe: %s", exePath);
	CreateProcessA(exePath, qaMode ? "-qa" : 0, 0, 0, FALSE, CREATE_NEW_PROCESS_GROUP, 0, 0, &si, &pi);
	ExitProcess(-100);
}