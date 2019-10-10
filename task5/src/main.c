#include <stdio.h>
#include <stdlib.h>
#include <math.h>	
#include <visa.h>

void main(int argc, char** argv)
{
	unsigned char resultBuffer[256];
	ViStatus status = VI_SUCCESS;
	ViFindList resourceList;
	ViUInt32 num_inst;
	ViUInt32 resultCount;

	ViSession defaultRM, scopeHandle;
	ViChar description[VI_FIND_BUFLEN];
	char dataBuffer[2500];
	float floatBuffer[2500];
	float y;

	int lsb;
	int msb;

	status = viOpenDefaultRM(&defaultRM);

	if(status == VI_SUCCESS)
	{
		status = viFindRsrc(defaultRM,"USB[0-9]::?*INSTR",	&resourceList,&num_inst,description);
		if(status == VI_SUCCESS)
		{
			status = viOpen(defaultRM,description,	VI_NULL,VI_NULL,&scopeHandle);

			if(status == VI_SUCCESS)
			{
				printf("\nOpened %s\n",description);

				viWrite(scopeHandle,"*IDN?\n",6,&resultCount);
				viRead(scopeHandle,resultBuffer,256,&resultCount);

				printf("\nResult count = %d",resultCount);
				printf("\nResult buffer = %s\n",resultBuffer);

				//viWrite(scopeHandle,"CH1:SCA?\n",9,&resultCount);	// Do we need this line?
				//viRead(scopeHandle,dataBuffer,250,&resultCount);

				viWrite(scopeHandle,"DAT:SOU CH1\n",12,&resultCount);
				viWrite(scopeHandle,"DAT:START 1\n",12,&resultCount);
				viWrite(scopeHandle,"DAT:STOP 2500\n",14,&resultCount);
				viWrite(scopeHandle,"CURV?\n",6,&resultCount);
				_sleep(2);
				viRead(scopeHandle,dataBuffer,2500,&resultCount);
				/*
				if(status == VI_SUCCESS){
					char ret[36];		// return
					/* Then we need read the string into a floating point value using sscanf */
					/*
					for(int k = 0; k < something; k++){
						sscanf(ret,"%f",volts);
					}
				}
				*/
				for(int i = 0; i<1000; i++)
				{
					floatBuffer[i] = dataBuffer[i];
					y = floatBuffer[i] * 8.0 / 256; 		
					printf("\nVoltage = %f",y);
				}
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