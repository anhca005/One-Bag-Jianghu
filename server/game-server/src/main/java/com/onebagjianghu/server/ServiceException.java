package com.onebagjianghu.server;

/** Expected request failure carrying a protocol error code. */
final class ServiceException extends RuntimeException
{
    private final String code;

    ServiceException(String code, String message)
    {
        super(message);
        this.code = code;
    }

    String code()
    {
        return code;
    }
}
