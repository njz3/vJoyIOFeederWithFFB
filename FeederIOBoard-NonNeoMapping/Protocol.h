/*
  1 byte text protocol
*/
#pragma once
#include "Config.h"

namespace Protocol {

void SetupPort();
void DebugMessageFrame(String debug);
void SendStatusFrame();
void SendMessageFrame(String msg);
void SendErrorFrame(int code, String msg);
int ProcessOneMessage();

}
