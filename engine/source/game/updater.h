#pragma once

class Updater
{	
	static void ApplyUpdate(const char* updateDir);

public:
	static void Init(const char** argv, int argc);
};