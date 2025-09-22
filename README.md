# APC

#### Introduction

This POC demonstrates the new undocumented syscall `NtQueueApcThreadEx2`, used for APC code injection. By default, the thread must be in an alertable state. However, with `QUEUE_USER_APC_FLAGS_SPECIAL_USER_APC`, this requirement can be bypassed. Futhermore, this project only NT APIs + Direct Syscalls.

#### Signature

```c
NTSTATUS
NtQueueApcThreadEx2(
    IN HANDLE ThreadHandle,
    IN HANDLE UserApcReserveHandle,
    IN QUEUE_USER_APC_FLAGS QueueUserApcFlags,
    IN PPS_APC_ROUTINE ApcRoutine,
    IN PVOID SystemArgument1 OPTIONAL,
    IN PVOID SystemArgument2 OPTIONAL,
    IN PVOID SystemArgument3 OPTIONAL
    );
```

#### What to expect?

<p align="center">
  <img src="https://i.imgur.com/9NnIkjY.png" alt="drawing"/> 
</p>

```c#

Win32.RtlInitUnicodeString(
    out Win32.UNICODE_STRING us,
    @"\??\Z:\Syscalls\apc\msgbox64.bin"
);
```

#### To-Do

- [ ] Implement `ntdll!NtCreateThreadEx`
- [ ] Avoid `ntdll!NtAllocateVirtualMemory`
