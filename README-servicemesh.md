# Azure AKS and Service Meshes

Select Service Mesh you want to install

https://docs.microsoft.com/en-us/azure/aks/servicemesh-about

## Deploy Linkerd

[Linkerd](https://docs.microsoft.com/en-us/azure/aks/servicemesh-linkerd-about) is an easy to use and **lightweight service mesh**.

How to install Linkerd follow this [steps](https://docs.microsoft.com/en-us/azure/aks/servicemesh-linkerd-install?pivots=client-operating-system-linux).

### Install Linkerd library

```bash
# Specify the Linkerd version that will be leveraged throughout these instructions
LINKERD_VERSION=stable-2.6.0
curl -sLO "https://github.com/linkerd/linkerd2/releases/download/$LINKERD_VERSION/linkerd2-cli-$LINKERD_VERSION-linux"
sudo cp ./linkerd2-cli-$LINKERD_VERSION-linux /usr/local/bin/linkerd
sudo chmod +x /usr/local/bin/linkerd
```

Check linkerd command and check installation.

```bash
linkerd check --pre
```

### Deploy Linkerd into AKS

```bash
linkerd install | kubectl apply -f -
kubectl get svc --namespace linkerd --output wide
kubectl get pod --namespace linkerd --output wide
```

And check installation and run Dashboard

```bash
linkerd check

linkerd dashboard
```

![Linkerd Dashboard](media/linkerd-dashboard.png)

## Deploy Linkerd to my jjweb

Deploy website to AKS as described in projects.

Next inject Linkerd into deployment in Default namespace.
Is not supported on Windows nodes.

```bash
kubectl get -n default deploy -o yaml \
  | linkerd inject - \
  | kubectl apply -f -

linkerd dashboard
```

![Linkerd Dashboard](media/linkerd-jjweb.png)
