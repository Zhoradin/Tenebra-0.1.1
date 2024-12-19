public enum CardType
{
    Aqua,
    Gale,
    Gloom,
    Ember,
    Dust,
    Holy,
    Abyss
}

public static class TypeEffectiveness
{
    public static readonly float[,] effectivenessMatrix = new float[,]
    {
        /*
        // Aqua, Gale, Gloom, Ember, Dust, Holy, Normal
        { 1f, 1f, 1f, 1f, 1f, 1f, 1f }, // Aqua
        { 1f, 1f, 1f, 1f, 1f, 1f, 1f }, // Gale
        { 1f, 1f, 1f, 1f, 1f, 1f, 1f }, // Gloom
        { 1f, 1f, 1f, 1f, 1f, 1f, 1f }, // Ember
        { 1f, 1f, 1f, 1f, 1f, 1f, 1f }, // Dust
        { 1f, 1f, 1f, 1f, 1f, 1f, 1f },  // Holy
        { 1f, 1f, 1f, 1f, 1f, 1f, 1f }    // Abyss */

        
        // Aqua, Gale, Gloom, Ember, Dust, Holy, Normal
        { .5f, 1f, 2f, .5f, 2f, 1f, 1f }, // Aqua
        { 1f, .5f, 1f, 2f, .5f, 2f, 1f }, // Gale
        { .5f, 1f, .5f, 2f, 1f, 2f, 1f }, // Gloom
        { 2f, .5f, 1f, .5f, 2f, 1f, 1f }, // Ember
        { 2f, 2f, .5f, 1f, .5f, 1f, 1f }, // Dust
        { 1f, 2f, 2f, 1f, 1f, .5f, 1f },  // Holy
        { 1f, 1f, 1f, 1f, 1f, 1f, 1f }    // Abyss 
    };

    public static float GetEffectiveness(CardType attacker, CardType defender)
    {
        int row = (int)attacker;
        int col = (int)defender;
        return effectivenessMatrix[row, col];
    }
}

