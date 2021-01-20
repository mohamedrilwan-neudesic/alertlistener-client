import React from 'react';

import Message from './Message/Message';


const ChatWindow = (props) => {
    console.log("Chat",props)
    const chat = props.chat
        .map(m => 
        <Message 
            key={Date.now() * Math.random()}
            user={m.user}
            message={m.message}
            level={m.level}/>);
    return(
        <div>
            {chat}
        </div>
    )
};

export default ChatWindow;