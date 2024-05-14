
public class PlayerModifiers
{
    int healthAddition = 0;
    float damageAddition = 0f;
    float damagemultiplier = 1f;
    float attackSpeed = 1f;

    public static PlayerModifiers operator +(PlayerModifiers a, PlayerModifiers b)
    {
        PlayerModifiers result = new PlayerModifiers();
        result.healthAddition = a.healthAddition + b.healthAddition;
        result.damageAddition = a.damageAddition + b.damageAddition;
        result.damagemultiplier = a.damagemultiplier + b.damagemultiplier;
        result.attackSpeed = a.attackSpeed + b.attackSpeed;

        return result;
    }

}

