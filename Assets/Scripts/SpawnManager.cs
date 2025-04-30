using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour {
    // Variables
    [SerializeField] public Transform[] spawnPoints; // list of spawn point locations
    public DebrisFactory debrisFactory; // the factory that creates debris objects
    [SerializeField] private int numberOfDebrisToSpawn = 8; // # of debris to spawn in the scene
    [SerializeField] private int numberOfDebrisTypeDupes = 2; // # of debris type duplicates
    [SerializeField] private int numberOfSpawnPoints = 12; // # of spawn points set in the scene // Do not decrease this number, only increase this number if you make more spawn points
    [SerializeField] private int updatedLengthOfArray = 0; // Check if any debris types have been taken 
    // The new length to the array is synthetically updated.
    // The length of the available debris is retrieved from the last index after a debris type is not available. 
    // This makes the leftmost index(s) seen, the rightmost is the length, and the middle is skipped/ignored (however long).

    // Dictionaries
    private Dictionary<int, Transform> spawnPointDictionary; // saves the spawn point locations so they can be randomly selected by number
    private Dictionary<DebrisType, int> keepTrackOfTypes; // holds the number of times each debris type has been chosen

    // Private Array
    private int[] availableDebris; // holds the integer values for the available debris types

    /* Start is called once before the first execution of Update after the MonoBehaviour is created */
    void Start() {
        // Set the initial availableDebris and keepTrackOfTypes
        InitializeDebrisTypesTrackers();

        // Initialize spawn points into the Dictionary and Spawn debris
        InitializeSpawnPoints();
        SpawnDebris();
    }//end Start()

    /* Initialize the array of available debris and the Dictionary that keeps track of how many times a debris type was picked */
    private void InitializeDebrisTypesTrackers() {
        for (int i = 0; i < 4; i++) {
            availableDebris[i] = i;
            keepTrackOfTypes.Add((DebrisType)i, 0);
        }//end for-loop
    }//end InitializeDebrisTypesTrackers()

    /* Put the spawn points (just a number representing it) and their position into a Dictionary */
    private void InitializeSpawnPoints() {
        // Check if the array holding the spawn points has the correct max number
        if (spawnPoints.Length != numberOfSpawnPoints) {
            Debug.LogError($"Exactly {numberOfSpawnPoints} spawn points must be assigned.");
            return;
        }//end if

        // Fill the spawn point dictionary
        spawnPointDictionary = new Dictionary<int, Transform>();
        for (int i = 0; i < spawnPoints.Length; i++) {
            spawnPointDictionary.Add(i + 1, spawnPoints[i]);
        }//end for-loop
    }//end InitializeSpawnPoints()

    /* A Method to randomly spawn debris at randomly selected spawn points */
    private void SpawnDebris() {
        // Check if the debrisFactory exists for this script
        if (debrisFactory == null) {
            Debug.LogError("DebrisFactory not assigned");
            return;
        }//end if

        // (An update that could be made after we go into production would be setting the number to spawn if it is greater the spawn points. For now it can't happen.)
        // Check if there are enough spawn points set
        if (numberOfDebrisToSpawn > spawnPointDictionary.Count) {
            Debug.LogError("Not enough spawn points for the number of debris.");
            return;
        }//end if

        // Iterate 12 times
        for (int i = 0; i < numberOfDebrisToSpawn; i++) {
            // Get a random spawn point
            int randomKey = GetRandomSpawnPointKey();

            // Check if there are spawn points available // This shouldn't happen but would break the system if it were aloud to happen
            if (randomKey == -1) {
                Debug.LogError("No spawn points available.");
                break;
            }//end for-loop

            // Get the spawn point location
            Transform spawnPoint = spawnPointDictionary[randomKey];
            spawnPointDictionary.Remove(randomKey);
            
            // Get a random debris type from the available types
            DebrisType randomType = GetRandomDebrisType();
            UpdateAvailableDebris(randomType);

            // Create a debris with the debris factory // Instantiate a Debris object at the spawn point
            debrisFactory.CreateDebris(randomType, spawnPoint.position, Quaternion.identity);
        }//end for-loop
    }//end SpawnDebris()

    /* Get a random spawn point then remove it from the choices/Dictionary */
    private int GetRandomSpawnPointKey() {
        // Check if the Dictionary is empty
        if (spawnPointDictionary.Count == 0) {
            return -1;
        }//end if

        // Create an array to hold the keys
        int[] keys = new int[spawnPointDictionary.Count];

        // Copy only the keys into the array
        spawnPointDictionary.Keys.CopyTo(keys, 0);

        // Return a random key
        return keys[Random.Range(0, keys.Length)];
    }//end GetRandomSpawnPointKey()

    /* Get a random debris type from the remaining types still available to be picked */
    private DebrisType GetRandomDebrisType() {
        DebrisType randomType;
        int lengthUpdate;

        // Check if there is only one debris type left
        if (availableDebris.Length == 1) {
            // Set the last type manually
            randomType = (DebrisType)availableDebris[0];
        } else {
            // Set the length to the full array or the trimmed array length and get a random type from the list
            lengthUpdate = SetLengthOfAvailableDebrisArray(updatedLengthOfArray);
            randomType = (DebrisType)Random.Range(availableDebris[0], availableDebris[lengthUpdate]);
        }//end if-else

        // Update the count for the selected debris type
        keepTrackOfTypes[randomType] += 1;

        return randomType;
    }//end GetRandomDebrisType()

    /* Helper function to set the number of available debris types array length */
    private int SetLengthOfAvailableDebrisArray(int updatedLengthOfArray) {
        int length;

        // Set the length of the "end" to the array that is being visible to the script
        if (updatedLengthOfArray == 1) { // this is the first time here
            length = availableDebris[availableDebris.Length - 1] - 1;
        } else { // this is every other time
            // Get the length from the last index in the array
            length = availableDebris.Length - 1;
        }//end if-else

        return length;
    }//end SetLengthOfAvailableDebrisArray()

    /* Remove the debris type that has been chosen for the max amount of times */
    private int[] UpdateAvailableDebris(DebrisType randomType) {
        if (keepTrackOfTypes[randomType] == numberOfDebrisTypeDupes) {
            int intOfType = (int)randomType;
            int length = SetLengthOfAvailableDebrisArray(updatedLengthOfArray);
            
            // Loop through the array for the length
            for (int j = 0; j < length; j++) {
                // Check if the index is equal to the integer value of the type being removed
                if (availableDebris[j] == intOfType) {
                    // Set the value to the next value in the array
                    availableDebris[j] = availableDebris[j+1];
                } else {
                    // Check if the current count is equal to the last value set in the array
                    if (j != 0)  {
                        if (availableDebris[j] == availableDebris[j - 1]) {
                            // Set the value at the current index to the next value number
                            availableDebris[j] = availableDebris[j+1]; // example, take away zero, then 1 goes to the 0 index and 1 is still in the index 1
                        }//end if
                    }//end if

                    // Else the value stays the same
                }//end if-else
            }//end for-loop

            // Set the last value to the trimmed length of the array
            availableDebris[availableDebris.Length - 1] = availableDebris.Length - 1;
            updatedLengthOfArray = 1;
        }//end if

        return availableDebris;
    }//end UpdateAvailableDebris()
}//end SpawnManager
