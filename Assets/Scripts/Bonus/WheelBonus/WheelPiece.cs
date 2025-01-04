using UnityEngine ;

namespace EasyUI.PickerWheelUI {
   [System.Serializable]
   public class WheelPiece {
      public Sprite Icon ;
      [HideInInspector] public int Index ;
      [HideInInspector] public double _weight = 0f ;
   }
}
