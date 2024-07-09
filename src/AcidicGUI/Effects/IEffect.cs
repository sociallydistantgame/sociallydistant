namespace AcidicGUI.Effects;

public interface IEffect
{
    int PassesCount { get; }
    
    void Use(int pass);
    void UpdateOpacity(float opacity);
}