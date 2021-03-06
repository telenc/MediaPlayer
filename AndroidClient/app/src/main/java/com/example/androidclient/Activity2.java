package com.example.androidclient;

import android.app.Activity;
import android.content.Intent;
import android.os.AsyncTask;
import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.TextView;
import java.io.BufferedWriter;
import java.io.IOException;
import java.io.OutputStreamWriter;
import java.net.Socket;
import java.net.SocketException;
import java.net.UnknownHostException;

public class Activity2 extends Activity {
    private static Button           buttonLecture, buttonPause, buttonStop, buttonNext, buttonPrev, buttonDeco;
    private static TextView         monitor;
    private static Socket           mySocket;
    private static BufferedWriter   toServer;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_activity2);
        buttonLecture = (Button) findViewById(R.id.LectureButton);
        buttonPause = (Button) findViewById(R.id.pauseButton);
        buttonStop = (Button) findViewById(R.id.StopButton);
        buttonNext = (Button) findViewById(R.id.nextButton);
        buttonPrev = (Button) findViewById(R.id.prevButton);
        buttonDeco = (Button) findViewById(R.id.decoButton);
        monitor = (TextView) findViewById(R.id.monitorTextView);
        buttonLecture.setOnClickListener(sendCmdTcp);
        buttonPause.setOnClickListener(sendCmdTcp);
        buttonStop.setOnClickListener(sendCmdTcp);
        buttonNext.setOnClickListener(sendCmdTcp);
        buttonPrev.setOnClickListener(sendCmdTcp);
        buttonDeco.setOnClickListener(sendCmdTcp);
        Intent myIntent = getIntent();
        MyClientTask myClientTask = new MyClientTask(myIntent.getStringExtra("hostname"));
        myClientTask.execute();
        monitor.setText("Connexion at " + myIntent.getStringExtra("hostname"));
    }

    class MyClientTask extends AsyncTask<Void, Void, Void> {
        String dstAddress;

        MyClientTask(String addr) {
            dstAddress = addr;
        }

        @Override
        protected Void doInBackground(Void... arg0) {
            try {
                mySocket = new Socket(dstAddress, 4242);
//                mySocket = new Socket("192.168.1.88", 4242);
                toServer = new BufferedWriter(new OutputStreamWriter(mySocket.getOutputStream()));
            } catch (UnknownHostException e1) {
                e1.printStackTrace();
            } catch (IOException e) {
                e.printStackTrace();
            }
            return null;
        }
    }

    public OnClickListener sendCmdTcp =
            new OnClickListener(){
                @Override
                public void onClick(View arg) {
                    if (arg == buttonDeco)
                        try {
                            toServer.close();
                            finish();
                        } catch (IOException e) {
                            e.printStackTrace();
                        }
                    if (mySocket.isBound() == true) {
                        try {
                            if (arg == buttonLecture)
                                toServer.write("play");
                            else if (arg == buttonPause)
                                toServer.write("pause");
                            else if (arg == buttonStop)
                                toServer.write("stop");
                            else if (arg == buttonNext)
                                toServer.write("suivant");
                            else if (arg == buttonPrev)
                                toServer.write("precedent");
                            else if (arg == buttonDeco)
                                toServer.write("fuckoff");
                            toServer.flush();
                        } catch (IOException e) {
                            e.printStackTrace();
                        }
                    }
                    else
                        monitor.setText("Socket close");
                }};
}
