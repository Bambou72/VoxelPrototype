# Conventions:

- ## Code naming in the engine
    - Class names must have the first char as upper.
        ```csharp
        //Bad
        class example
        {
            
        }

        //Good
        class Example
        {
            
        }
        ```
    - Method parameters names must have the first char as upper.
        ```csharp
        //Bad
        void Test(int integer)
        {
        }

        //Good
        void Test(int Integer)
        {
        }
        ```
    - Always use block of code {} and never use tabulation.
        ```csharp
        //Bad
        if(Exemple)
            Example();

        //Good
        if(Exemple)
        {
            Example();
        }
        ```

- ## Asset Naming
    - Assets must have there name with only lower case.  
      e.g. with stairs block model the name is **stairs**.
