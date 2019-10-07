CC		= gcc
CFLAGS	= -c
LFLAGS	= 

#Specify build, include and source directories
BUILDDIR  = ./build
INCDIR    = ./includes
SRCDIR    = ./src

LIBDIR = ./build

#Create a list of all the .c files in the source directory
SOURCES   = $(wildcard $(SRCDIR)/*.c)
#Make another list by replacing all the .c's from the sources list with .o's and change the directory to build
OBJECTS   = $(patsubst $(SRCDIR)/%.c, $(BUILDDIR)/%.o, $(SOURCES))

LIBRARIES  = visa64

LIBCMD    = $(addprefix -l,$(LIBRARIES))

TARGET	  = program

$(BUILDDIR)/%.o: $(SRCDIR)/%.c
	$(CC) $(CFLAGS) -I$(INCDIR) -o $@ $<

$(TARGET): $(OBJECTS)
	$(CC) -o $@ $^ $(LIBCMD)