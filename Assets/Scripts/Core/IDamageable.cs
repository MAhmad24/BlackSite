/// <summary>
/// Any object that can receive damage implements this interface.
/// Bullets, explosions, traps — anything that deals damage — can target
/// any IDamageable without knowing what it is.
/// </summary>
public interface IDamageable
{
    void TakeDamage(int damage);
    bool IsDead { get; }
}
