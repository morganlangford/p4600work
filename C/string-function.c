#include <stdio.h>
#include <string.h>
/*
void Display(char ch[]);	//fu

int main()
{
	char c[50];
	printf("Enter string: ");
	gets(c);
	Display(c);	//prints output using 'puts'
	return 0;
}

void Display(char ch[])
{
	printf("String Output: ");
	puts(ch);
}
*/

int main()
{
	char str1[12] = "Hello";
	char str2[12] = "World";
	char str3[12];
	int len;

	strcpy(str3,str1);	// string copy. Copy str1 to str3
	printf("strcpy str3: %s\n",str3);

	strcat(str1,str2);	// add str2 to str1
	printf("strcat(str1,str2): %s\n",str1);

	len = strlen(str1); // string length (returns 10)
	printf("strlen(str1): %d\n",len);

	return 0;
}