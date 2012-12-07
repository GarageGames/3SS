
COMPILER_OPTIONS=CW6 VC6 GCC2 GCC3 GCC3.4 GCC4
BUILD_OPTIONS=DEBUG RELEASE
OS_OPTIONS=WIN32 BEOS LINUX OpenBSD FreeBSD Solaris

OS=
DIR.OBJ=out

-include conf.mk
CONFIG_STATUS=VALID 

ifeq ($(findstring $(OS), $(OS_OPTIONS)),)
	targets += badOS
CONFIG_STATUS=INVALID
endif

ifeq ($(findstring $(COMPILER), $(COMPILER_OPTIONS)),)
	targets +=badCompiler
CONFIG_STATUS=INVALID
endif

ifeq ($(findstring $(BUILD), $(BUILD_OPTIONS)),)
	targets += badBuild
CONFIG_STATUS=INVALID
endif

default: $(targets) print save

badOS:
	@echo ERROR: OS variable not set or is an illegal value

badCompiler:
	@echo ERROR: COMPILER variable not set or is an illegal value

badBuild:
	@echo ERROR: BUILD variable not set or is an illegal value

print:
	@echo  
	@echo "Current Configuration: this config is $(CONFIG_STATUS)"
	@echo "         OS: $(OS)"
	@echo "   COMPILER: $(COMPILER)"
	@echo "      BUILD: $(BUILD)"   
	@echo "    DIR.OBJ: $(DIR.OBJ)"   
	@echo  
	@echo "To change the current configuration type:"
	@echo  
	@echo "make -f configure.mk {arguments, ...}"
	@echo  
	@echo "required arguments:"
	@echo "  OS={$(OS_OPTIONS)}"
	@echo "  COMPILER={$(COMPILER_OPTIONS)}"
	@echo "  BUILD={$(BUILD_OPTIONS)}"
	@echo  
	@echo "optional arguments:"
	@echo "  DIR.OBJ={path to store intermediate obj files}"
	@echo  
	@echo "Note: all arguments are case sensitive." 
	@echo  

save:
	@echo OS=$(OS) > conf.mk
	@echo COMPILER=$(COMPILER) >> conf.mk
	@echo BUILD=$(BUILD) >> conf.mk

	@echo ifeq \"$(BUILD)\" \"DEBUG\" >> conf.mk
	@echo BUILD_SUFFIX:=_DEBUG >> conf.mk
	@echo else >> conf.mk
	@echo BUILD_SUFFIX:= >> conf.mk
	@echo endif >> conf.mk

	@echo CONFIG_STATUS=$(CONFIG_STATUS) >> conf.mk
	@echo DIR.OBJ=$(DIR.OBJ) >> conf.mk

	@echo ifndef COMPILER_OPTIONS >> conf.mk
	@echo DIR.OBJ:=$(DIR.OBJ).$(COMPILER).$(BUILD) >> conf.mk
	@echo endif >> conf.mk
