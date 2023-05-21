## Samples

This directory contains sample server that upon database record changed (created, updated, deleted) it publishes the audit data to `rabbitmq`.
On the other "side" a receiver fetches messages from the queue and handle these.
