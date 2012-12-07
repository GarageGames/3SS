#!/bin/sh

pushd ../
source common.sh
popd

# Make sure the documentation directory exists
if [ ! -d "$DOCUMENTATIONDIR" ]; then
   echo "Error: documentation directory does not exist!"
   exit 1
fi

# Build the engine docs.
echo "Generating engine doxygen docs..."
pushd "$TOOLSDIR/doxygen"
#./generateEngineDocs.sh

# And the console docs.
echo "Generating console doxygen docs..."
/bin/bash ./generateConsoleDocs.sh
popd

# And run the docGenerator.
echo "Generating documentation with buildSystemScheme..."
pushd "$TOOLSDIR/docGenerator"
# /bin/bash ./generateDocs.sh buildSystemScheme
popd
