#include <stdio.h>
#include <stdlib.h>
#include <math.h>	
#include <visa.h>

#include "amplitude.h"
#include "smoothCurve.h"

void main(int argc, char** argv)
{
	unsigned char resultBuffer[256];		// holds input values
	ViStatus status = VI_SUCCESS;
	ViFindList resourceList;
	ViUInt32 num_inst;
	ViUInt32 resultCount;

	ViSession defaultRM, scopeHandle;
	ViChar description[VI_FIND_BUFLEN];
	char dataBuffer[2500];
	float floatBuffer[2500];
	float y;
	float input[1000];
	float output[1000];

	int lsb;
	int msb;

	status = viOpenDefaultRM(&defaultRM);	
	if(status == VI_SUCCESS)
	{
		status = viFindRsrc(defaultRM,"USB[0-9]::?*INSTR",&resourceList,&num_inst,description);
		if(status == VI_SUCCESS)
		{
			status = viOpen(defaultRM,description,	VI_NULL,VI_NULL,&scopeHandle);
			if(status == VI_SUCCESS)
			{
				printf("\nOpened %s\n",description);

				// Ask for the IDN number
				viWrite(scopeHandle,"*IDN?\n",6,&resultCount);
				viRead(scopeHandle,resultBuffer,256,&resultCount);

				printf("\nResult count = %d",resultCount);
				printf("\nResult buffer = %s\n",resultBuffer);

				viWrite(scopeHandle,"DAT:SOU CH1\n",12,&resultCount);
				viWrite(scopeHandle,"DAT:START 1\n",12,&resultCount);
				viWrite(scopeHandle,"DAT:STOP 2500\n",14,&resultCount);

				/* Get curve (data points) */ 
				ViUInt32 resultCount;
				ViStatus status;
				viWrite(scopeHandle, "CURV?\n",6,&resultCount);
				status = viRead(scopeHandle, dataBuffer, 2500, &resultCount); 

				/* Get scale of scope */
				char ret[20];	// return
				ret[19] = '\0';
				viWrite(scopeHandle,"CH1:SCA?\n",9,&resultCount);
				viRead(scopeHandle,ret,20,&resultCount);
				printf("Scale is %s\n",ret);

				float volts;
				sscanf(ret,"%f",&volts);
				printf("\nvolts = %f",volts);

				for(int i = 0; i < 1000; i++)
				{
					floatBuffer[i] = dataBuffer[i];
					y = floatBuffer[i] * 10.0 * volts / 256; 	
					input[i] = y;	
					printf("\n%f",y);
				}

				/* Now we need to actually smooth the curve and find the amplitude */
				printf("\nSmooth curve data:\n");
				smoothCurve(1000, input, output);
				for (int j = 0; j < 1000; j++){
					printf("\n%f", output[j]);
				}				

				float amp = amplitude(1000, output);
				printf("\nAmplitude = %f",amp);
			}
			else
			{
				printf("\nFailed to open %s",description);
			}
		}
		else
		{
			printf("\nCouldn't find any instruments");
		}
	}
	else
	{
		printf("\nFailed to open defaultRM");
	}
}