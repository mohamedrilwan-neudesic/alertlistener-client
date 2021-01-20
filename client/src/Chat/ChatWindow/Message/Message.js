import React from "react";
import Alert from "@material-ui/lab/Alert";



const Message = (props) => (
  

  <div style={{ background: "#eee", borderRadius: "5px", padding: "0 10px" }}>
    <p>
      <strong>{props.user}</strong>
    </p>
    <Alert variant="filled" severity={props.level === "high" ? "error" : props.level === "severe" ? "error" : props.level === "moderate" ? "warning" : "info"}>
      {props.message}
    </Alert>
  </div>
);

export default Message;
