using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Play.Review;

public class IAPManager : Singleton<IAPManager>
{
    
    private ReviewManager _reviewManager;
    private PlayReviewInfo _playReviewInfo;
    // Start is called before the first frame update
    private void Start()
    {
    }
    public void Call()
    {
        StartCoroutine(RequestReview());
    }
    IEnumerator RequestReview()
    {
        
        _reviewManager = new ReviewManager();
        var requestFlowOperation = _reviewManager.RequestReviewFlow();
        yield return requestFlowOperation;
        if (requestFlowOperation.Error != ReviewErrorCode.NoError)
        {
            Debug.LogError("Error GG Review");
            // Log error. For example, using requestFlowOperation.Error.ToString().
            yield break;
        }
        _playReviewInfo = requestFlowOperation.GetResult();

        var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);
        yield return launchFlowOperation;
        _playReviewInfo = null; // Reset the object
        if (launchFlowOperation.Error != ReviewErrorCode.NoError)
        {
            Debug.LogError("Error GG Review");
            // Log error. For example, using requestFlowOperation.Error.ToString().
            yield break;
        }
        Debug.LogError("Request Google Review");
        // The flow has finished. The API does not indicate whether the user
        // reviewed or not, or even whether the review dialog was shown. Thus, no
        // matter the result, we continue our app flow.
    }
}
