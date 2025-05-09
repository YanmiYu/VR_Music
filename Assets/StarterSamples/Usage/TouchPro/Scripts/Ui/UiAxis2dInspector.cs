/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Licensed under the Oculus SDK License Agreement (the "License");
 * you may not use the Oculus SDK except in compliance with the License,
 * which is provided at the time of installation or download, or which
 * otherwise accompanies this software in either electronic or hard copy form.
 *
 * You may obtain a copy of the License at
 *
 * https://developer.oculus.com/licenses/oculussdk/
 *
 * Unless required by applicable law or agreed to in writing, the Oculus SDK
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Meta.XR.Samples;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[MetaCodeSample("StarterSample-TouchPro")]
public class UiAxis2dInspector : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Vector2 m_xExtent = new Vector2(-1, +1);

    [SerializeField] private Vector2 m_yExtent = new Vector2(-1, +1);

    [Header("Components")]
    [SerializeField] private TextMeshProUGUI m_nameLabel = null;

    [SerializeField] private TextMeshProUGUI m_valueLabel = null;
    [SerializeField] private Image m_handle = null;

    public void SetExtents(Vector2 xExtent, Vector2 yExtent)
    {
        m_xExtent = xExtent;
        m_yExtent = yExtent;
    }

    public void SetName(string name)
    {
        m_nameLabel.text = name;
    }

    public void SetValue(bool isTouching, Vector2 value)
    {
        m_handle.color = isTouching ? Color.white : new Color(0.2f, 0.2f, 0.2f);

        Vector2 clampedValue = new Vector2(
            Mathf.Clamp(value.x, m_xExtent.x, m_xExtent.y),
            Mathf.Clamp(value.y, m_yExtent.x, m_yExtent.y)
        );

        m_valueLabel.text = $"[{clampedValue.x.ToString("f2")}, {clampedValue.y.ToString("f2")}]";

        RectTransform parentRect = m_handle.transform.parent.GetComponent<RectTransform>();
        Vector2 parentSize = (parentRect != null)
            ? new Vector2(Mathf.Abs(parentRect.sizeDelta.x), Mathf.Abs(parentRect.sizeDelta.y))
            : new Vector2(Mathf.Abs(m_xExtent.y - m_xExtent.x), Mathf.Abs(m_yExtent.y - m_yExtent.x));

        m_handle.transform.localPosition = new Vector3(
            clampedValue.x * parentSize.x * 0.5f,
            clampedValue.y * parentSize.y * 0.5f,
            0
        );
    }
}
