#include <stdio.h>
#include <stdlib.h>
#include <math.h>

int main(){
	/* */
	FILE* sineFile;

	float sineWave[64];	

	sineFile = fopen("sineWave.dat","w"); //open file called data.dat, r means read
	
	for (int i=0; i<64; i++){
		sineWave[i] = sin((3.14159265 * i)/8);
		fprintf(sineFile,"\n%f %f",(3.14159265*i/8),sineWave[i]);
	}

	fclose(sineFile);

	return 0;
}