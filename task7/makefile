MCU=atmega328p
AVRDUDEMCU=m328p
#Needed for windows compile
TOOLCHAIN=/c/'Program Files (x86)'/Atmel/Studio/7.0/toolchain/avr8/avr8-gnu-toolchain/bin/
#TOOLCHAIN=
CC=$(TOOLCHAIN)avr-gcc
THIS_ADDRESS=0x1010
DEFINES=-DF_CPU=1000000UL -DMY_ADDRESS=$(THIS_ADDRESS)
CFLAGS=-g $(DEFINES) -Os -Wall -mcall-prologues -mmcu=$(MCU)
OBJ2HEX=$(TOOLCHAIN)avr-objcopy
AVRDUDE=avrdude
#Change this to your programmer COM port
PORT=COM4
#PORT=/dev/ttyACM0
PROGRAMMER=stk500v2

# User directories to search
BUILDDIR  = ./build
INCDIR    = ./include
SRCDIR    = ./src

# 3rd Party library directories to search
LIBDIR    = 
LIBINCDIR = 

# List sources, object files and libraries to be used
SOURCES   = $(wildcard $(SRCDIR)/*.c)
OBJECTS   = $(patsubst $(SRCDIR)/%.c, $(BUILDDIR)/%.o, $(SOURCES))
LIBRARIES = 

# Create command line arguments
LIBCMD    = $(addprefix -l,$(LIBRARIES))
LIBDIRCMD = $(addprefix -L,$(LIBDIR))
INCCMD    = $(addprefix -I,$(INCDIR) $(LIBINCDIR) $(COMMONINCDIR)) 


TARGET=$(MCU)

all: $(TARGET).hex

$(BUILDDIR)/%.o: $(SRCDIR)/%.c
	$(CC) -c $(CFLAGS) $(INCCMD) $< -o $@
	
$(TARGET).elf: $(OBJECTS) 
	$(CC) $(CFLAGS) $(OBJECTS)  -o $(TARGET).elf
	
$(TARGET).hex: $(TARGET).elf	 	
	$(OBJ2HEX) -R .eeprom -O ihex $(TARGET).elf $(TARGET).hex
	
install: all
	$(AVRDUDE) -c $(PROGRAMMER) -P "$(PORT)" -p $(MCU) -b 10000 -U flash:w:$(TARGET).hex
	
fuse:
	$(AVRDUDE) -c $(PROGRAMMER) -P "$(PORT)" -p $(MCU) -b 10000 -U hfuse:w:0xd9:m -U efuse:w:0x07:m

clean:
	rm -f *.hex *.obj *.elf $(BUILDDIR)/*.o