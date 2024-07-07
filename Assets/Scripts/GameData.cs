using UnityEngine;

public static class GameData
{
    public enum BuildType { Damage, Fitness, Adrenaline }
    public static MainMenu.PowerUpPair SelectedPowerUpPair { get; set; }
    public static BuildType CurrentBuild { get; set; }
}
