import * as React from "react";
import { useState } from 'react';
// import sendGreeting from "../../main";
const { ConnectionBuilder } = require("electron-cgi");


export const Dashboard = () => {
  let [cssResponse, setCsResponse] = useState("");
  let [lmaoString, setLmaoString] = useState("");

  let connection = new ConnectionBuilder()
  .connectTo("dotnet", "run", "--project", "C:/proj/Pauli/client/backend/FrontConsole")
  .build();

  connection.onDisconnect = () => {
    console.log("lost");
  };

  const sendGreeting = () => {
    connection.send('greeting', 'John', (error: any, theGreeting: any) => {
    if (error) {
        console.log(error); //serialized exception from the .NET handler
        return;
    }
    setCsResponse(theGreeting);
    console.log(theGreeting);
  });
  }

  const dostuff = () => {
    setLmaoString("LMAOO")
  }
  return (
    <div>
      Hello Mom! This is Pauli.
      <p></p>
      <button onClick={dostuff}>Do stuff</button>
      <p>{lmaoString}</p>
       <button onClick={sendGreeting}>Say Hi</button>
      <p>{cssResponse}</p>
      {/*
      <button onClick={() => connection.close()}>Close</button> */}
    </div>
  );
};
