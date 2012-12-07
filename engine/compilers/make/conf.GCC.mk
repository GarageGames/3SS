

CFLAGS    = $(CFLAGS.GENERAL) $(CFLAGS.$(BUILD))
LFLAGS    = $(LFLAGS.GENERAL) $(LFLAGS.$(BUILD))
LINK.LIBS = $(LINK.LIBS.GENERAL) $(LINK.LIBS.$(BUILD))
MAKE.DIR = ../compilers/make/


define DO.COMPILE.C 
	@echo "--> Compiling $(subst $(SOURCE.DIR),,$(<))"
	@cd $(SOURCE.DIR); $(COMPILER.c) $(CFLAGS) -I. $(PATH.H.SYS) -c -o $(MAKE.DIR)$(@) $(subst $(SOURCE.DIR),,$(<))
endef

define DO.COMPILE.CC 
	@echo "--> Compiling $(subst $(SOURCE.DIR),,$(<))"
	@cd $(SOURCE.DIR); $(COMPILER.cc) $(CFLAGS) -I. $(PATH.H.SYS) -c -o $(MAKE.DIR)$(@) $(subst $(SOURCE.DIR),,$(<))
endef

define DO.COMPILE.ASM
	@echo "--> Assembling $(subst $(SOURCE.DIR),,$(<))"
	@cd $(SOURCE.DIR); $(COMPILER.asm) $(ASMFLAGS) -o $(MAKE.DIR)$(@) $(subst $(SOURCE.DIR),,$(<))
endef

define DO.LINK.CONSOLE.EXE
	@echo "--> Linking $@"
	@$(COMPILER.cc) -o $@ $(LFLAGS) $(LINK.SOURCES) $(LINK.LIBS)
endef

define DO.LINK.LIB
	@echo "Creating library $@"
	@$(LINK) -cr $@ $(LINK.SOURCES)
endef
