.SUFFIXES:
.SUFFIXES: $(O) $(EXE) $(DLL) .S .c .cc .h .asm .ash .y .cpp

DIR.LIST=$(DIR.OBJ)/ $(addprefix $(DIR.OBJ)/, $(sort $(dir $(SOURCE.ALL))))


LINK.SOURCES=$(filter %$(O),$^)
LINK.RESOURCES=$(filter %.res,$^)
LINK.DEFS=$(filter %.def,$^)


.PHONY: dirlist 
#vpath %$(O) $(DIR.OBJ)

$(DIR.OBJ)/%$O : $(SOURCE.DIR)%.cpp
	$(DO.COMPILE.CC)

$(DIR.OBJ)/%$O : $(SOURCE.DIR)%.cc
	$(DO.COMPILE.CC)

$(DIR.OBJ)/%$O : $(SOURCE.DIR)%.c
	@$(DO.COMPILE.C)

$(DIR.OBJ)/%$O : $(SOURCE.DIR)%.asm
	@$(DO.COMPILE.ASM)

$(DIR.OBJ)/%.res : $(SOURCE.DIR)%.rc
	@$(DO.COMPILE.RC)

dirlist: $(DIR.LIST)

$(DIR.LIST):
	@$(if $(wildcard $@),, $(MKDIR))

clean: $(targetsclean)
	$(RMDIR) $(DIR.OBJ)/
