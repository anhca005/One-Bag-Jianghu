package com.onebagjianghu.server;

import javax.crypto.SecretKeyFactory;
import javax.crypto.spec.PBEKeySpec;
import java.security.MessageDigest;
import java.security.SecureRandom;

/** PBKDF2 password hashing with protocol constants. */
public final class PasswordHasher
{
    static final int ITERATIONS = 120_000;
    private final SecureRandom random;

    public PasswordHasher(SecureRandom random)
    {
        this.random = random;
    }

    public byte[] newSalt()
    {
        byte[] salt = new byte[16];
        random.nextBytes(salt);
        return salt;
    }

    public byte[] hash(char[] password, byte[] salt)
    {
        PBEKeySpec spec = new PBEKeySpec(password, salt, ITERATIONS, 256);
        try
        {
            return SecretKeyFactory.getInstance("PBKDF2WithHmacSHA256").generateSecret(spec).getEncoded();
        }
        catch (Exception exception)
        {
            throw new IllegalStateException("PBKDF2 is unavailable", exception);
        }
        finally
        {
            spec.clearPassword();
        }
    }

    public boolean matches(char[] password, byte[] salt, byte[] expected)
    {
        return MessageDigest.isEqual(expected, hash(password, salt));
    }
}
