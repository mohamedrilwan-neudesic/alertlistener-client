import React, { useState, useEffect, useRef } from "react";
import { HubConnectionBuilder } from "@microsoft/signalr";

import ChatWindow from "./ChatWindow/ChatWindow";
import ChatInput from "./ChatInput/ChatInput";
import { Box, Button, Typography } from "@material-ui/core";
import axios from "axios";

const Chat = () => {
  const [connection, setConnection] = useState(null);
  const [chat, setChat] = useState([]);
  const [allId, setAllId] = useState([]);
  const latestChat = useRef(null);
  const latestUser = useRef(null);
  const [user, setUser] = useState("");

  latestChat.current = chat;
  latestUser.current = allId;

  useEffect(() => {
    const username = prompt("Please enter your name");
    setUser(username);
    const url = `https://localhost:44327/alert`;
    const newConnection = new HubConnectionBuilder()
      .withUrl("https://localhost:44327/alert")
      .withAutomaticReconnect()
      .build();
    setConnection(newConnection);
  }, []);

  useEffect(() => {
    if (connection) {
      connection
        .start()
        .then((result) => {
          console.log("Connected!");
          connection.on("Broadcast", (user, level, message) => {
            console.log("Broadcast", user, message, level);
            const updatedChat = [...latestChat.current];
            updatedChat.push({ user, level, message });
            setChat(updatedChat);
          });
          connection.on("Echo", (user, message) => {
            console.log("Echo", user, message);
            // const updatedChat = [...latestChat.current];
            // updatedChat.push({ user, level, message });
            // setChat(updatedChat);
          });
          connection.on("OnConnected", (newId) => {
            console.log(newId);
            const updatedUser = [...latestUser.current];
            updatedUser.push(newId);
            setAllId(updatedUser);
            console.log("us", updatedUser);
            console.log("All User", updatedUser);
          });
        })
        .catch((e) => console.log("Connection failed: ", e));
    }
  }, [connection]);

  function createGuid() {
    return "xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx".replace(
      /[xy]/g,
      function (c) {
        var r = (Math.random() * 16) | 0,
          v = c === "x" ? r : (r & 0x3) | 0x8;
        return v.toString(16);
      }
    );
  }

  function joinGroup(){
    const groupName=prompt("Enter group name")
    connection.invoke("JoinGroup", user, groupName);
  }

  const sendMessage = async (
    user,
    message,
    level,
    sendToType,
    specificUser,
    groupMessage,
    groupName
  ) => {
    const chatMessage = {
      user: user,
      message: message,
      level: level,
      specificUser: specificUser,
      groupMessage: groupMessage,
      groupName: groupName,
    };

    if (connection.connectionStarted) {
      try {
        if (sendToType === "broadcast")
          await axios.post(
            "https://localhost:44345/Notifications/publish/alert",
            {
              correlationId: createGuid(),
              alertMessage: {
                sender: {
                  name: user,
                  email: "string",
                },
                broadcast: true,
                sendToSpecificUsers: false,
                sendToGroup: false,
              },
              level: chatMessage.level,
              message: chatMessage.message,
            }
          );

        if (sendToType === "self")
          await connection.invoke(
            "BroadcastToCaller",
            user,
            chatMessage.message,
            chatMessage.level
          );

        if (sendToType === "user")
          await axios.post(
            "https://localhost:44345/Notifications/publish/alert",
            {
              correlationId: createGuid(),
              alertMessage: {
                sender: {
                  name: user,
                  email: "string",
                },
                broadcast: false,
                sendToSpecificUsers: true,
                recipients: [
                  {
                    name: "rilwan",
                    email: "string",
                  },
                  {
                    name: "arjun",
                    email: "string",
                  },
                ],
                sendToGroup: false,
              },
              level: chatMessage.level,
              message: chatMessage.message,
            }
          );

        if (sendToType === "messagegroup") {
          debugger;
          await axios.post(
            "https://localhost:44345/Notifications/publish/alert",
            {
              correlationId: createGuid(),
              alertMessage: {
                sender: {
                  name: user,
                  email: "string",
                },
                broadcast: false,
                sendToSpecificUsers: true,
                recipients: [
                  {
                    name: "rilwan",
                    email: "string",
                  },
                  {
                    name: "arjun",
                    email: "string",
                  },
                ],
                sendToGroup: true,
                groupName:groupName
              },
              level: chatMessage.level,
              message: chatMessage.message,
            }
          );
        }

        if (sendToType === "joingroup")
          await connection.invoke("JoinGroup", user, "groupName");
      } catch (e) {
        console.log(e);
      }
    } else {
      alert("No connection to server yet.");
    }
  };

  return (
    <div style={{padding:"100px",alignContent:"center",textAlign: "center", border: "3px solid green"}}>
      <Box >
      <Typography variant="h5" color="primary" style={{paddingBottom:"10px"}}>Hi {user}</Typography>
      <ChatInput sendMessage={sendMessage} allUser={allId} />
     
      <Button style={{margin:"20px"}} variant="contained" color="primary" onClick={joinGroup}>Join Group</Button>
      <ChatWindow chat={chat} />
      </Box>
    </div>
  );
};

export default Chat;
