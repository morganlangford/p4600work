#include <stdio.h>

int main()
{
	char c[10];

	printf("Enter the name:\n");
	gets(c);
	printf("Name is %s\n",c);
	puts(c); //prints string


	return 0;
}