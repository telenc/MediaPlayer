using System;

public class SpeechEvent
{
	public SpeechEvent(MediaElement mediaElement)
	{
        this.myMediaElement = mediaElement;
	}

    void    playFunc()
    {
        this.myMediaElement.SpeedRation = 1;
        this.myMediaElement.Play();
    }

    MediaElement myMediaElement;
}
