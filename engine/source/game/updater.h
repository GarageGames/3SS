#pragma once

class Updater
{	
	static void ApplyUpdate(const char* updateDir, bool qaMode);

public:
	static void Init(const char** argv, int argc);
};