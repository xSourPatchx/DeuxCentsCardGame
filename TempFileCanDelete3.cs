

### 2. Set File Properties
Right-click `appsettings.json` → Properties → Set "Copy to Output Directory" to **"Copy if newer"**

Or add to `.csproj`:
```xml
<ItemGroup>
  <None Update="appsettings.json">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
  </None>
</ItemGroup>
```

### 3. Create New Configuration Classes
Create `GameConfig/GameSettings.cs` with all the configuration models (already provided in artifacts).

### 4. Update Existing Files

#### GameConfig.cs
- Change from using `GameConstants` to using `GameSettings`
- Constructor now takes `GameSettings` parameter
- Properties delegate to nested settings objects

#### Program.cs
- Add configuration builder
- Load and bind `appsettings.json`
- Pass `gameConfig` to components that need it
- Pass `gameConfig.InitialDealerIndex` to `BettingManager`

#### UIGameView.cs
- Add `GameConfig` dependency in constructor
- Replace `GameConstants` usage with `_gameConfig` properties

#### GameEventHandler.cs
- Change `IGameConfig` to `GameConfig` (to access UI properties)
- Replace constant usage with `_gameConfig` properties

#### GameController.cs
- Remove hard-coded `INITIAL_DEALER_INDEX`
- Get initial dealer from `BettingManager`

### 5. Optional: Keep GameConstants for Card Validation
You can keep `GameConstants.cs` for card-specific validation constants that shouldn't be configurable:
- `MINIMUM_CARD_FACE_VALUE`
- `MAXIMUM_CARD_FACE_VALUE`
- `CARD_POINT_VALUE_*`

These are game rules, not configuration.

## Benefits

✅ **Easy Configuration**: Change game rules without recompiling
✅ **Multiple Environments**: Different settings for dev/test/prod
✅ **Runtime Changes**: Reload configuration without restart (if enabled)
✅ **Validation**: Centralized validation of settings
✅ **Documentation**: JSON schema can document valid values

## Testing Different Configurations

Create multiple config files:
- `appsettings.json` - Default
- `appsettings.Development.json` - For testing
- `appsettings.QuickGame.json` - Faster games (lower winning score)

Load based on environment:
```csharp
.AddJsonFile("appsettings.json", optional: false)
.AddJsonFile($"appsettings.{environment}.json", optional: true)
```

## Migration Checklist

- [ ] Create `appsettings.json`
- [ ] Create `GameSettings.cs` model classes
- [ ] Update `GameConfig.cs` to use `GameSettings`
- [ ] Update `Program.cs` with configuration builder
- [ ] Update `UIGameView.cs` constructor
- [ ] Update `GameEventHandler.cs` to use `GameConfig`
- [ ] Update `GameController.cs` dealer initialization
- [ ] Set `appsettings.json` to copy to output directory
- [ ] Test that game runs with new configuration
- [ ] (Optional) Remove unused `GameConstants` entries

## Troubleshooting

**File not found**: Ensure `appsettings.json` is set to "Copy if newer"

**Null values**: Check JSON property names match C# class properties exactly

**Invalid values**: Add validation in `GameConfig` constructor if needed