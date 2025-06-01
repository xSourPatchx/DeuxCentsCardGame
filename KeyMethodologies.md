### **Key methodologies**

    - **MVC/MVP**: Separate game logic from display/input for easier Unity porting-
    - **State Pattern/FSM**: Manage game flow states (waiting, dealing, player turns, etc.)
    - **Command Pattern**: Encapsulate player actions for undo/redo and network sync
    - **Event-Driven Architecture**: Handle multiplayer notifications and game events
    - **Dependency Injection**: Swap between AI, human, and network players easily
    - **Core Logic Separation**: Keep game rules in plain C# classes, not Unity-dependent code