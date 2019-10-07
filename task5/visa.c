/*
For programming Tektonics oscilloscope

*IDN?  - gets the unique ID of that oscilloscope 

CH2VOL  - sets voltage for channel 2 

Everything you can do on on the oscillosope on the front panel
you can do by using a command/program

You can read the data directly 

first thing we want to do:
*/

#include <stdio.h>
#include <stdlib.h>
#include <math.h>
#include <visa.h> 	// don't have this installed yet


void main(int argc, char ** argv){
	char inq[] = "*IDN?\n";			// check its unique ID
	ViStatus status = VI_SUCCESS;
	ViFindList resourceList;		// place to put what you find, send later
	ViUInt32 num_inst;

	ViUInt32 returnCount = 0;

	ViSession defaultRM, scopeHandle;
	ViChar description[VI_FIND_BUFLEN];

	char idnResponse[1024];
	char message[1024];

	status = viOpenDefaultRM(&defaultRM);

	// Did that work?
	if(status == VI_SUCCESS){		// if yes, then...
		status = viFindRsrc(defaultRM,"USB[0-9]::*INSTR",&resourceList,&num_inst,description);		
		/* This finds resources (instruments), any instrument that's USB 
		connected and it's an actual instrument called "search string",
		send the list, how many things did it find, description*/
		if(status == VI_SUCCESS){
			status = viOpen(defaultRM,description,VI_NULL,VI_NULL,&scopeHandle);
			// assuming this all went okay, we should be connected to the scope. Check.
			if(status == VI_SUCCESS){
				prinf("Opened %s",description);		
			}
			else printf("Failed to open %s",description);
		}	
		else printf("Couldn't find any instruments");
	}
	else printf("Failed to open defaultRM");
}