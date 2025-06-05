## **Key methodologies**

### **MVP (Model-View-Presenter)**: The Presenter acts as an intermediary that completely decouples View from Model. The View is "dumb" - it only displays data and forwards user actions to the Presenter. The Presenter handles all logic, updates the Model, and tells the View exactly what to display. MVP gives you more control over exactly when and how your UI updates, which is crucial for card games where timing and visual feedback matter. No direct Model-View communication. For your Unity card game: MVP tends to work better because:
- Unity's component-based UI makes "dumb" Views natural
- Easier to unit test game logic (Presenters don't depend on MonoBehaviours)
- Cleaner networking integration (Presenters can handle multiplayer state changes)
- Better separation when you have complex card interactions and animations
### **Overview**:
- **Model**: Same as MVC (game data).  
- **View**: Handles visuals but **delegates logic to Presenter**.  
- **Presenter**: Acts as a middleman—handles input, updates Model, and refreshes View.  
- **Flow**: **User Input → View → Presenter → Model → View**  
- **Best for**: More complex UI interactions (e.g., multiplayer card games with heavy UI logic).  


### **State Pattern/FSM**: Manage game flow states (waiting, dealing, player turns, etc.)

### **Command Pattern**: Encapsulate player actions for undo/redo and network sync

### - **Event-Driven Architecture**: Handle multiplayer notifications and game events

### - **Dependency Injection**: Swap between AI, human, and network players easily

### - **Core Logic Separation**: Keep game rules in plain C# classes, not Unity-dependent code