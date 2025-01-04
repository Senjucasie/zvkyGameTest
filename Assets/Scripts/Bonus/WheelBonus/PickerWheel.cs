using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;
using System.Collections.Generic;

namespace EasyUI.PickerWheelUI
{

   public class PickerWheel : MonoBehaviour
   {

      [Header("References :")]
      [SerializeField] private GameObject linePrefab;
      [SerializeField] private Transform linesParent;

      [Space]
      [SerializeField] private Transform PickerWheelTransform;
      [SerializeField] private Transform wheelCircle;
      [SerializeField] private GameObject wheelPiecePrefab, WheelSpinButton;
      [SerializeField] private GameObject showbuffalo;
      [SerializeField] private Transform wheelPiecesParent;

      [Space]
      [Header("Sounds :")]
      [SerializeField] private AudioSource audioSource;
      [SerializeField] private AudioClip tickAudioClip;

      [Space]
      [Header("Picker wheel settings :")]
      [Range(1, 20)] public int WheelSpinDuration = 8;

      [Space]
      [Header("Picker wheel pieces :")]
      public WheelPiece[] wheelPieces;

      // Events
      private UnityAction onSpinStartEvent;
      private UnityAction<WheelPiece> onSpinEndEvent;


      private bool _isSpinning = false;

      public bool IsSpinning { get { return _isSpinning; } }


      private Vector2 pieceMinSize = new(81f, 146f);
      private Vector2 pieceMaxSize = new(144f, 213f);
      private int piecesMin = 2;
      private int piecesMax = 12;

      private float pieceAngle;
      private float halfPieceAngle;

      private readonly double accumulatedWeight;

      private List<int> nonZeroChancesIndices = new();

      public Transform[] pieces = new Transform[6];
      private void Start()
      {
         pieceAngle = 360 / wheelPieces.Length;
         halfPieceAngle = pieceAngle / 2f;
         showbuffalo.SetActive(false);
         WheelSpinButton.SetActive(true);

         Generate();

         CalculateWeightsAndIndices();

         SetupAudio();

      }

      private void SetupAudio()
      {
         // audioSource.clip = tickAudioClip;
         // audioSource.volume = volume;
         // audioSource.pitch = pitch;
      }

      private void Generate()
      {
         wheelPiecePrefab = InstantiatePiece();

         RectTransform rt = wheelPiecePrefab.transform.GetChild(0).GetComponent<RectTransform>();
         float pieceWidth = Mathf.Lerp(pieceMinSize.x, pieceMaxSize.x, 1f - Mathf.InverseLerp(piecesMin, piecesMax, wheelPieces.Length));
         float pieceHeight = Mathf.Lerp(pieceMinSize.y, pieceMaxSize.y, 1f - Mathf.InverseLerp(piecesMin, piecesMax, wheelPieces.Length));
         rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, pieceWidth);
         rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, pieceHeight);

         for (int i = 0; i < wheelPieces.Length; i++)
            DrawPiece(i);

         Destroy(wheelPiecePrefab);
      }

      private void DrawPiece(int index)
      {
         WheelPiece piece = wheelPieces[index];
         Transform pieceTrns = InstantiatePiece().transform.GetChild(0);

         pieceTrns.GetChild(0).GetComponent<Image>().sprite = piece.Icon;
         pieces[index] = pieceTrns;

         //Line
         Transform lineTrns = Instantiate(linePrefab, linesParent.position, Quaternion.identity, linesParent).transform;
         lineTrns.RotateAround(wheelPiecesParent.position, Vector3.back, (pieceAngle * index) + halfPieceAngle);
         pieceTrns.RotateAround(wheelPiecesParent.position, Vector3.back, pieceAngle * index);
      }

      private GameObject InstantiatePiece()
      {
         return Instantiate(wheelPiecePrefab, wheelPiecesParent.position, Quaternion.identity, wheelPiecesParent);
      }


      public void Spin()
      {
         Audiomanager.Instance.PlayUiSfx(SFX.Button_Click);
         if (!_isSpinning)
         {
            Audiomanager.Instance.PlaySfx(SFX.Wheelspin);
            _isSpinning = true;
            onSpinStartEvent?.Invoke();
            showbuffalo.SetActive(true);
            WheelSpinButton.SetActive(false);
            int index = 0;

            WheelPiece piece = wheelPieces[index];

            if (nonZeroChancesIndices.Count != 0)
            {
               // index = nonZeroChancesIndices[Random.Range(0, nonZeroChancesIndices.Count)];
               switch (GameApiManager.Instance.ApiData.GetWheelBonusReward())
               {
                  case "MINI":
                     index = 4;
                     break;
                  case "MAJOR":
                     index = 2;
                     break;
                  case "GRAND":
                     index = 0;
                     break;
                  case "MULTIPLIER_3":
                     index = 1;
                     break;
                  case "MULTIPLIER_5":
                     index = 3;
                     break;
                  case "MULTIPLIER_8":
                     index = 5;
                     break;

               }
               piece = wheelPieces[index];
            }

            float angle = -(pieceAngle * index);

            Vector3 targetRotation = Vector3.back * (angle + 2 * 360 * WheelSpinDuration);

            float prevAngle, currentAngle;

            prevAngle = currentAngle = wheelCircle.eulerAngles.z;

            bool isIndicatorOnTheLine = false;

            wheelCircle
            .DORotate(targetRotation, WheelSpinDuration, RotateMode.Fast)
            .SetEase(Ease.InOutQuart)
            .OnUpdate(() =>
            {
               float diff = Mathf.Abs(prevAngle - currentAngle);

               if (diff >= halfPieceAngle)
               {
                  if (isIndicatorOnTheLine)
                  {
                     //audioSource.PlayOneShot(audioSource.clip);
                  }
                  prevAngle = currentAngle;
                  isIndicatorOnTheLine = !isIndicatorOnTheLine;
               }
               currentAngle = wheelCircle.eulerAngles.z;
            })
            .OnComplete(() =>
            {
               _isSpinning = false;
               onSpinEndEvent?.Invoke(piece);
               onSpinStartEvent = null;
               onSpinEndEvent = null;
            });

         }
      }


      public void OnSpinStart(UnityAction action)
      {
         onSpinStartEvent = action;
      }

      public void OnSpinEnd(UnityAction<WheelPiece> action)
      {
         onSpinEndEvent = action;
         //double amount = GameApiManager.Instance.ApiData.GetWheelBonusWinAmount();
         double amount = GameApiManager.Instance.ApiData.GetWheelBonusFinalWinAmount();
         Debug.Log("Bonus Amount Won" + amount);
      }

      private void CalculateWeightsAndIndices()
      {
         for (int i = 0; i < wheelPieces.Length; i++)
         {
            WheelPiece piece = wheelPieces[i];

            piece._weight = accumulatedWeight;

            piece.Index = i;

            nonZeroChancesIndices.Add(i);
         }
      }
   }
}