
#if TODO_STATION_CONTACTS

namespace StationeersWebApi.Payloads
{
    public class TradingItemPayload
    {
        public float tradeValue;
        public float maxQuantity;
        public int itemSerialNumber;
        public float quantity;
        public int prefabHash;
        public int quantityToPurchase;
        public float damageStateRatio;
        public int numberOfItemsStocked;

        // TODO: Serialize / deserialize as string.
        public TraderItemType type;

        public static TradingItemPayload FromTradingItemDat(TradingItemDat dat)
        {
            var payload = new TradingItemPayload
            {
                tradeValue = dat.TradeValue,
                maxQuantity = dat.MaxQuantity,
                itemSerialNumber = dat.ItemSerialNumber,
                quantity = dat.Quantity,
                prefabHash = dat.PrefabHash,
                quantityToPurchase = dat.QuantityToPurchase,
                damageStateRatio = dat.DamageStateRatio,
                numberOfItemsStocked = dat.NumberOfItemsStocked,
                type = dat.type,
                // TODO: Send gas mixture
            };
            return payload;
        }
    }
}
#endif