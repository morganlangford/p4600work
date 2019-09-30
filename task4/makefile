# Makefile is like a macro for running your command line 

# make a list of all the .o files that are in the directory
OBJ = main.o smoothCurve.o amplitude.o 
BUILDDIR = ./build
SRCDIR = ./src
INCLUDEDIR = ./include
OBJDIR = $(patsubst %,$(BUILDDIR)/%,$(OBJ))

TARGET = program

#main.o: main.c
#	gcc -c -o main.o main.c

#smoothCurve.o: smoothCurve.c
#	gcc -c -o smoothCurve.o smoothCurve.c

# but that's a lot of work to type all of them out so... 
$(BUILDDIR)/%.o: $(SRCDIR)/%.c
	gcc -c -o $@ $< 	# where $@ means everything to the left of the : and $< means to the right
# -o means : "output file name"

# if this is out of date, it'll do the above (like if anything has changed)
# it links the file objects into an executable 
$(TARGET): $(OBJ)
	gcc -o $@ $^  	# recompile all of the above ones 