package net.laparola.ui.android.dialogs;

import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.EditText;

import net.laparola.R;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.fragment.app.Fragment;

public class ReferenceTabFragment extends Fragment {
    private EditText referenceEditText;

    public ReferenceTabFragment() {}

    @Nullable
    @Override
    public View onCreateView(@NonNull LayoutInflater inflater,
                             @Nullable ViewGroup container,
                             @Nullable Bundle savedInstanceState) {
        View view = inflater.inflate(R.layout.fragment_reference_tab, container, false);

        referenceEditText = view.findViewById(R.id.reference_edittext);

        return view;
    }

    public EditText getReferenceEditText() {
        return referenceEditText;
    }
}