-include conf.mk

ifndef CONFIG_STATUS
doConfigure: 
	$(error Configuration file not defined.  Please run $(MAKE) -f configure.mk)
	#@$(MAKE) --no-print-directory -f configure.mk
else
ifeq ($(CONFIG_STATUS),INVALID)
doConfigure:
	$(error Invalid Configuration file.  Please run $(MAKE) -f configure.mk)
	#@$(MAKE) --no-print-directory -f configure.mk
endif
endif

default:
	@$(MAKE) -s -f MakefileLib default
	@$(MAKE) -s -f MakefileEngine

.PHONY: engine dedicated docs clean 

dedicated:
	@$(MAKE) -s -f MakefileLib
	@$(MAKE) -s -f MakefileEngine dedicated

engine:
	@$(MAKE) -s -f MakefileLib 
	@$(MAKE) -s -f MakefileEngine engine

html_docs:
# 	Assumes Doxygen is in your path
	@doxygen doc/doxygen/html/doxygen.html.cfg

php_docs:
#  Internal gg use only
# 	Assumes Doxygen is in your path
	@doxygen doc/doxygen/php/doxygen.php.cfg

all: default

clean:
	@$(MAKE) -s -f MakefileEngine clean
	@$(MAKE) -s -f MakefileLib clean
