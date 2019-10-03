#include <stdio.h>
#include <stdlib.h>
#include <math.h>	
#include <visa.h>

void main(int argc, char** argv)
{
	char inq[] = "*IDN?/n";
	ViStatus status = VI_SUCCESS;
	ViFindList resourceList;
	ViUInt32 num_inst;

	ViSession defaultRM, scopeHandle;
	ViChar description[VI_FIND_BUFLEN];

	status = viOpenDefaultRM(&defaultRM);

	if(status == VI_SUCCESS)
	{
		status = viFindRsrc(defaultRM,"USB[0-9]::0?*INSTR",
						&resourceList,&num_inst,description);
		if(status == VI_SUCCESS)
		{
			status = viOpen(defaultRM,description,
							VI_NULL,VI_NULL,&scopeHandle);

			if(status == VI_SUCCESS)
			{
				printf("Opened %s",description);
			}
			else
			{
				printf("Failed to open %s",description);
			}
		}
		else
		{
			printf("Couldn't find any instruments");
		}
	}
	else printf("Failed to open defaultRM");
}