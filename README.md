### Brief Introduction

This **Lua scripting system** is part of my ETC project [HelloAlice](jinyidai.squarespace.com/projects/hello-alice).

HelloAlice is an AI-powered creative coding tool that transforms natural language prompts into real-time updates within a 3D scene. 

Since the tool is built in Unity 6 and C# is not well-suited for real-time compilation and execution, I designed and developed this Lua scripting layer that bridges the gap. This layer exposes wrapped C# functions to Lua, enabling safe and dynamic code execution in Unity runtime environment.

### Structure

    /Lua
      - LuaManager.cs             // Manager for
      - LuaScript.cs              // Container for full scripts
      /Codebase
        - ILuaMethod.cs           // Interface for Lua methods
        - LuaMethod.cs            // Base implementation
        - LuaCodebase.cs          // Executes game-related logic
        /LuaMethods               // Lua method subclass
          - MoveObjectMethod.cs   // Moves an object to a position
          - PlaySoundMethod.cs    // Plays a sound effect
          ...
          /ParameterType
            - IParameter.cs       // Generic interface & base class for Lua method parameters
            - FloatParameter.cs   // Example concrete parameter class
            - StringParameter.cs  // ...
