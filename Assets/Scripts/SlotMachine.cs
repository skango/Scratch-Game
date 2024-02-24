using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SlotMachine : MonoBehaviour
{
    public Sprite[] symbols; // Array of symbols to display on the slot machine
    public Image[] reels; // Array of Image components representing the reels
    public Button spinButton; // Spin button to initiate the spinning

    private bool isSpinning = false; // Flag to indicate whether the reels are currently spinning

    void Start()
    {
        spinButton.onClick.AddListener(SpinReels); // Subscribe to the SpinReels method when the spin button is clicked
    }

    private void Update()
    {
        if (isSpinning)
        {
            for (int i = 0; i < reels.Length; i++)
            {
                int randomSymbolIndex = Random.Range(0, symbols.Length);
                reels[i].sprite = symbols[randomSymbolIndex];
            }
        }
    }

    void SpinReels()
    {
        if (!isSpinning)
        {
            isSpinning = true;
            spinButton.interactable = false; // Disable the spin button while spinning

            // Generate random symbols for each reel
            for (int i = 0; i < reels.Length; i++)
            {
                int randomSymbolIndex = Random.Range(0, symbols.Length);
                reels[i].sprite = symbols[randomSymbolIndex];
            }

            // Start spinning animation (you can implement your own spinning animation here)
            StartCoroutine(SpinAnimation());
        }
    }

    IEnumerator SpinAnimation()
    {
        // Implement your spinning animation here (e.g., rotating the reels)

        // Wait for a few seconds to simulate spinning
        yield return new WaitForSeconds(2.0f);

        // After spinning, check for winning combinations (not implemented in this basic script)

        // Re-enable the spin button
        isSpinning = false;
        spinButton.interactable = true;
    }
}
