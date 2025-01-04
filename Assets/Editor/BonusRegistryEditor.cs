using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BonusFeaturesManager))]
public class BonusRegistryEditor : Editor
{
    private SerializedProperty _bonusListProperty;
    private SerializedProperty _bonusProperty;
    private SerializedProperty _isActiveProperty;

    private void OnEnable()
    {
        // Get the reference to the list property
        _bonusListProperty = serializedObject.FindProperty("_bonusList");
    }

    public override void OnInspectorGUI()
    {
        // Grab default values
        serializedObject.Update();

        // Draw the default inspector for the list property
        EditorGUILayout.PropertyField(_bonusListProperty, true);

        // Ensure only one bonus is active
        EnsureSingleActiveBonus(_bonusListProperty);

        // Apply Changes to the Editor
        serializedObject.ApplyModifiedProperties();
    }

    private void EnsureSingleActiveBonus(SerializedProperty _bonusListProperty)
    {
        bool _isActiveFound = false;
        
        for (int i = 0; i < _bonusListProperty.arraySize; i++)
        {
            _bonusProperty = _bonusListProperty.GetArrayElementAtIndex(i);
            _isActiveProperty = _bonusProperty.FindPropertyRelative("_isActive");

            if (_isActiveProperty.boolValue)
            {
                if (_isActiveFound)
                {
                    // If another bonus is already active, deactivate it
                    _isActiveProperty.boolValue = false;
                }
                else
                {
                    _isActiveFound = true;
                }
            }
            else if (!_isActiveFound && i == _bonusListProperty.arraySize - 1)
            {
                // If no active bonus is found and we're at the last element,
                // activate the current bonus
                _isActiveProperty.boolValue = true;
            }
        }
    }
}

