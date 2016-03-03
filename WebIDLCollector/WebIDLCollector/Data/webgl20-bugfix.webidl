interface WebGL2RenderingContextBase
{
  void deleteTransformFeedback(WebGLTransformFeedback? id);
  [WebGLHandlesContextLoss] GLboolean isTransformFeedback(WebGLTransformFeedback? id);
}