package com.example.androidclient;

import android.content.Intent;
import android.os.Bundle;
import android.app.Activity;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;

public class MainActivity extends Activity {

    TextView textResponse, infoSocket;
    EditText editTextAddress, editTextPort;
    Button buttonConnect, buttonClear, buttonOffline;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        editTextAddress = (EditText)findViewById(R.id.address);
        buttonConnect = (Button)findViewById(R.id.connect);
        buttonOffline = (Button)findViewById(R.id.offlineButton);
        infoSocket =  (TextView)findViewById(R.id.InfoSocket);
        buttonConnect.setOnClickListener(buttonConnectFunc);
        buttonOffline.setOnClickListener(showAtYourMom);
        infoSocket.setText("Initialisation");
    }

    OnClickListener showAtYourMom =         new OnClickListener() {
                @Override
                public void onClick(View view) {
                    startActivity(new Intent(getApplicationContext(), Activity2.class));}
            };

    OnClickListener buttonConnectFunc;    {
        buttonConnectFunc = new OnClickListener() {
            @Override
            public void onClick(View arg0) {
                Intent myIntent = new Intent(getApplicationContext(), Activity2.class);
                myIntent.putExtra("hostname", editTextAddress.getText().toString());
                startActivity(myIntent);
            }
        };
    }

}