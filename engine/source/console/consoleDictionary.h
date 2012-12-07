//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _CONSOLE_DICTIONARY_H_
#define _CONSOLE_DICTIONARY_H_

#ifndef _STRINGTABLE_H_
#include "string/stringTable.h"
#endif
#ifndef _VECTOR_H_
#include "collection/vector.h"
#endif
#ifndef _CONSOLETYPES_H_
#include "console/consoleTypes.h"
#endif

//-----------------------------------------------------------------------------

class ExprEvalState;
class CodeBlock;

extern char *typeValueEmpty;

//-----------------------------------------------------------------------------

class Dictionary
{
public:
    struct Entry
    {
        enum
        {
            TypeInternalInt = -3,
            TypeInternalFloat = -2,
            TypeInternalString = -1,
        };

        StringTableEntry name;
        Entry *nextEntry;
        S32 type;
        char *sval;
        U32 ival;  // doubles as strlen when type = -1
        F32 fval;
        U32 bufferLen;
        void *dataPtr;

        Entry(StringTableEntry name);
        ~Entry();

        U32 getIntValue()
        {
            if(type <= TypeInternalString)
                return ival;
            else
                return dAtoi(Con::getData(type, dataPtr, 0));
        }
        F32 getFloatValue()
        {
            if(type <= TypeInternalString)
                return fval;
            else
                return dAtof(Con::getData(type, dataPtr, 0));
        }
        const char *getStringValue()
        {
            if(type == TypeInternalString)
                return sval;
            if(type == TypeInternalFloat)
                return Con::getData(TypeF32, &fval, 0);
            else if(type == TypeInternalInt)
                return Con::getData(TypeS32, &ival, 0);
            else
                return Con::getData(type, dataPtr, 0);
        }
        void setIntValue(U32 val)
        {
            if(type <= TypeInternalString)
            {
                fval = (F32)val;
                ival = val;
                if(sval != typeValueEmpty)
                {
                    dFree(sval);
                    sval = typeValueEmpty;
                }
                type = TypeInternalInt;
                return;
            }
            else
            {
                const char *dptr = Con::getData(TypeS32, &val, 0);
                Con::setData(type, dataPtr, 0, 1, &dptr);
            }
        }
        void setFloatValue(F32 val)
        {
            if(type <= TypeInternalString)
            {
                fval = val;
                ival = static_cast<U32>(val);
                if(sval != typeValueEmpty)
                {
                    dFree(sval);
                    sval = typeValueEmpty;
                }
                type = TypeInternalFloat;
                return;
            }
            else
            {
                const char *dptr = Con::getData(TypeF32, &val, 0);
                Con::setData(type, dataPtr, 0, 1, &dptr);
            }
        }
        void setStringValue(const char *value);
    };

private:
    struct HashTableData
    {
        Dictionary* owner;
        S32 size;
        S32 count;
        Entry **data;
    };

    HashTableData *hashTable;
    ExprEvalState *exprState;

public:
    StringTableEntry scopeName;
    Namespace *scopeNamespace;
    CodeBlock *code;
    U32 ip;

    Dictionary();
    Dictionary(ExprEvalState *state, Dictionary* ref=NULL);
    ~Dictionary();
    Entry *lookup(StringTableEntry name);
    Entry *add(StringTableEntry name);
    void setState(ExprEvalState *state, Dictionary* ref=NULL);
    void remove(Entry *);
    void reset();

    void exportVariables(const char *varString, const char *fileName, bool append);
    void deleteVariables(const char *varString);

    void setVariable(StringTableEntry name, const char *value);
    const char *getVariable(StringTableEntry name, bool *valid = NULL);

    void addVariable(const char *name, S32 type, void *dataPtr);
    bool removeVariable(StringTableEntry name);

    /// Return the best tab completion for prevText, with the length
    /// of the pre-tab string in baseLen.
    const char *tabComplete(const char *prevText, S32 baseLen, bool);
};


#endif // _CONSOLE_DICTIONARY_H_
