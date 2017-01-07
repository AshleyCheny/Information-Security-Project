# Information-Security-Project

## Project Proposal

## Project Attack Tree

## We got it to work
https://ycandgap.me

Motivation and Problem Statement:
Signal implements a full end-to-end encryption security for users of its service. The open source protocol library in Java is available at https://github.com/whispersystems/libsignal-protocol-java/. However, the equivalent oﬃcial C# implementation of the library is not currently available.

Solution:
This project creates the C# implementation of the library and evaluate as a team, factors such as usability and security in message communication under the Signal protocol. 

Details of implementation: 
• Platform: The chat application is a web application. 
• Programming Language: The application includes the Signal protocol C# implemented library. The application itself is developed using .Net framework and Visual C# 
• Diﬀerent components involved in the project: The project involves the use of signal protocol implemented using a C# library. There is a possibility of this library being used as a web service. The application is a simple message transfer with limited size text messaging functionality. The application contains a server side implementation that assists in distributing the messages among the client applications. The use of encryption keys as speciﬁed in the signal protocol such as,
– Pubic Key Types: Identity Key Pair, Pre Key and One-Time Pre Key. 
– Session Key Types: Root Key, Chain Key and Message Key.
The implementation of the key structure and encryption mechanism such as Cure25519, Elliptical Curve Diﬃe-Hellman (ECDH), AES-256 and HMAC-SHA 256 will be integrated as part of the overall encryption mechanism. 
