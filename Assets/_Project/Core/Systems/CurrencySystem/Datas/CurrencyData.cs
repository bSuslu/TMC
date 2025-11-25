namespace _Project.Core.Systems.CurrencySystem.Datas
{
    [System.Serializable]
    public class CurrencyData
    {
        public bool IsUnlocked;
        public int Amount;
        
        public CurrencyData(bool isUnlocked, int amount)
        {
            IsUnlocked = isUnlocked;
            Amount = amount;
        }
    }
}