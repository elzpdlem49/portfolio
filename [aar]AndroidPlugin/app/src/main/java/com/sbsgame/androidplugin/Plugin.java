package com.sbsgame.androidplugin;

import android.app.Activity;
import android.app.AlertDialog;
import android.content.DialogInterface;
import android.widget.Toast;

public class Plugin {
    //플러그인 작동 테스트용 함수
    int GetInt()
    {
        return 20230829;
    }
    //엑티비티에 토스트메세지를 띄우기위해서 유니티에서 엑티비티를 설정하도록 매개변수로 사용한다.
    void ShowToastMsg(Activity activity, String msg, int mc_time)
    {
        Toast.makeText(activity,msg,mc_time).show();
    }
    //엑티비티에 토스트메세지를 띄우기위해서 유니티에서 엑티비티를 설정하도록 매개변수로 사용한다.
    //종료시 엑티비티의 finish를 호출하여 엑티비티 종료로 유니티를 종료한다.
    void ShowDialogExit(Activity activity, String msg, String title)
    {
        AlertDialog.Builder msgBuilder = new AlertDialog.Builder(activity)
                .setTitle(title)
                .setMessage(msg)
                .setPositiveButton("Ok", new DialogInterface.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialogInterface, int i) {
                        activity.finish();
                    }
                })
                .setNegativeButton("Cancle", new DialogInterface.OnClickListener() {
                    @Override
                    public void onClick(DialogInterface dialogInterface, int i) {

                    }
                });
        AlertDialog msgDlg = msgBuilder.create();
        msgDlg.show();
    }
}
