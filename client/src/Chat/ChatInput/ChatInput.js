import {
  Button,
  FormControl,
  InputLabel,
  MenuItem,
  Select,
  TextareaAutosize,
  TextField,
} from "@material-ui/core";
import React, { useState } from "react";

const ChatInput = (props) => {
  const [sendToType, setSendToType] = useState("");
  const [user, setUser] = useState("");
  const [message, setMessage] = useState("");
  const [level, setLevel] = useState("");
  const [group, setGroup] = useState("");
  const [specificUser, setSpecificUser] = useState("");
  const [groupMessage, setGroupMessage] = useState("");
  const [groupName, setGroupName] = useState("");

  const onSubmit = (e) => {
    e.preventDefault();

    const isUserProvided = user && user !== "";
    const isMessageProvided = message && message !== "";
    const isAlert = level && level !== "";

    if (isMessageProvided && isAlert) {
      props.sendMessage(
        user,
        message,
        level,
        sendToType,
        specificUser,
        groupMessage,
        groupName
      );
    } else {
      alert("Please insert an user and a message and type.");
    }
  };

  const onUserUpdate = (e) => {
    setUser(e.target.value);
  };

  const onMessageUpdate = (e) => {
    setMessage(e.target.value);
  };
  const onLevelUpdate = (e) => {
    setLevel(e.target.value);
  };
  const onSendToUpdate = (e) => {
    setSendToType(e.target.value);
  };

  const onGroupUpdate = (e) => {
    setGroup(e.target.value);
  };
  const onSpecificUserUpdate = (e) => {
    setSpecificUser(e.target.value);
  };

  return (
    <form onSubmit={onSubmit}>
      <TextField
        label="Message"
        value={message}
        style={{ marginBottom: "30px" }}
        variant="outlined"
        onChange={onMessageUpdate}
      ></TextField>
      <br />
      <FormControl
        variant="filled"
        style={{ width: "250px", marginBottom: "30px" }}
      >
        <InputLabel id="demo-simple-select-filled-label">Alert Type</InputLabel>
        <Select
          labelId="demo-simple-select-filled-label"
          id="demo-simple-select-filled"
          value={level}
          onChange={onLevelUpdate}
        >
          <MenuItem value={"normal"}>Normal</MenuItem>
          <MenuItem value={"moderate"}>Moderate</MenuItem>
          <MenuItem value={"high"}>High</MenuItem>
          <MenuItem value={"severe"}>Severe</MenuItem>
        </Select>
      </FormControl>
      <br />

      <FormControl
        variant="filled"
        style={{ width: "250px", marginBottom: "30px" }}
      >
        <InputLabel id="demo-simple-select-label">Send To Type</InputLabel>
        <Select
          labelId="sendto"
          id="sendTo"
          value={sendToType}
          onChange={onSendToUpdate}
        >
          <MenuItem value={"broadcast"}>Broadcast</MenuItem>
          <MenuItem value={"self"}>To Myself</MenuItem>
          <MenuItem value={"user"}>To Specific User</MenuItem>
          <MenuItem value={"messagegroup"}>Message Group</MenuItem>
        </Select>
      </FormControl>

      {sendToType === "user" ? (
        <FormControl variant="filled" style={{ width: "250px",marginLeft:"50px"}}>
          <InputLabel id="demo-simple-select-label">Select User</InputLabel>
          <Select
            labelId="user"
            id="user"
            value={specificUser}
            onChange={onSpecificUserUpdate}
          >
            {props.allUser.map((item) => (
              <MenuItem value={item}>{item}</MenuItem>
            ))}
          </Select>
        </FormControl>
      ) : null}

      {sendToType === "messagegroup" ? (
        <TextField
       
        style={{ marginLeft: "50px" }}
        variant="outlined"
        onChange={onMessageUpdate}
          label="Message to group"
          value={groupMessage}
          onChange={(e) => setGroupMessage(e.target.value)}
        />
      ) : null}

      <br />

      <br />
      <Button variant="contained" color="primary" onClick={onSubmit}>
        Send
      </Button>
    </form>
  );
};

export default ChatInput;
