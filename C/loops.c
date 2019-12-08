#include <stdio.h>
#include <string.h>

int main()
{
	int i;

	for(i = 0; i < 10; i++)
	{
		printf("%d\n",i);		// prints numbers 0 through 9
	}

	/*
	This is the same as 

	while (i<10)
	{
		printf("%d\n",i);
		i++;
	}

	and is similar to 

	do
	{
		printf("%d\n",i);
		i++;
	} while (i<10);

	but do while always executes once before checking condition
	*/
	return 0;
}
