/* 
Task 6:

Having one device connected:
Normally we write...

viFindRsrc(defaultRM,"USB[0-9]::?*INTSR",.....,description);
// but this will only find the first instr and open it
viOpen(defaultRM, description,.....,&scopeHandle);

Having multiple devices connected:
We want to open multiple instruments though 
So we'll write:

ViSession scopeHandle;
ViSession FGHandle;

viFindRsrc(defaultRM,"USB[0-9]::0x0699?*INTSR",.....,description); // tektronix
viOpen(defaultRM, description,.....,&scopeHandle);
Check status of search

viFindRsrc(defaultRM,"USB[0-9]::0x1AB1?*INTSR",.....,description); // rigol
viOpen(defaultRM, description,.....,&scopeHandle);
Check status of search

viWrite(scopeHandle,"*IDN?\n",6,....);
viWrite(FGHandle,"*IDN?\n",6,....);

*/